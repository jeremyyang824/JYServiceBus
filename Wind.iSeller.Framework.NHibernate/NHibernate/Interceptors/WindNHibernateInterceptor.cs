using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Domain.Entities;
using Wind.iSeller.Framework.Core.Domain.Entities.Auditing;
using Wind.iSeller.Framework.Core.Events.Bus;
using Wind.iSeller.Framework.Core.Events.Bus.Entities;
using Wind.iSeller.Framework.Core.Extensions;

namespace Wind.iSeller.Framework.NHibernate.Interceptors
{
    internal class WindNHibernateInterceptor : EmptyInterceptor
    {
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        private readonly IIocManager _iocManager;
        private readonly Lazy<IEventBus> _eventBus;

        public WindNHibernateInterceptor(IIocManager iocManager)
        {
            _iocManager = iocManager;

            _eventBus =
                new Lazy<IEventBus>(
                    () => _iocManager.IsRegistered(typeof(IEventBus))
                        ? _iocManager.Resolve<IEventBus>()
                        : NullEventBus.Instance,
                    isThreadSafe: true
                );
        }

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            return base.OnPrepareStatement(sql);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            //Set Id for Guids
            if (entity is IEntity<Guid>)
            {
                var guidEntity = entity as IEntity<Guid>;
                if (guidEntity.IsTransient())
                {
                    guidEntity.Id = Guid.NewGuid();
                }
            }

            //Set CreationTime for new entity
            if (entity is IHasCreationTime)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "CreationTime")
                    {
                        state[i] = (entity as IHasCreationTime).CreationTime = DateTime.Now;
                    }
                }
            }

            EntityChangeEventHelper.TriggerEntityCreatingEvent(entity);
            EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompleted(entity);

            TriggerDomainEvents(entity);

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            //Set modification audits
            if (entity is IHasModificationTime)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "LastModificationTime")
                    {
                        currentState[i] = (entity as IHasModificationTime).LastModificationTime = DateTime.Now;
                    }
                }
            }

            if (entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted)
            {
                //Is deleted before? Normally, a deleted entity should not be updated later but I preferred to check it.
                var previousIsDeleted = false;
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "IsDeleted")
                    {
                        previousIsDeleted = (bool)previousState[i];
                        break;
                    }
                }

                if (!previousIsDeleted)
                {
                    //set DeletionTime
                    if (entity is IHasDeletionTime)
                    {
                        for (var i = 0; i < propertyNames.Length; i++)
                        {
                            if (propertyNames[i] == "DeletionTime")
                            {
                                currentState[i] = (entity as IHasDeletionTime).DeletionTime = DateTime.Now;
                            }
                        }
                    }

                    EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);
                }
                else
                {
                    EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
                }
            }
            else
            {
                EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
            }

            TriggerDomainEvents(entity);

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
            EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);

            TriggerDomainEvents(entity);

            base.OnDelete(entity, id, state, propertyNames, types);
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            NormalizeDateTimePropertiesForEntity(state, types);
            return true;
        }

        private static void NormalizeDateTimePropertiesForEntity(object[] state, IList<IType> types)
        {
            for (var i = 0; i < types.Count; i++)
            {
                if (types[i].IsComponentType)
                {
                    NormalizeDateTimePropertiesForComponentType(state[i], types[i]);
                }

                if (types[i].ReturnedClass != typeof(DateTime) && types[i].ReturnedClass != typeof(DateTime?))
                {
                    continue;
                }

                var dateTime = state[i] as DateTime?;

                if (!dateTime.HasValue)
                {
                    continue;
                }

                state[i] = dateTime.Value;
            }
        }

        private static void NormalizeDateTimePropertiesForComponentType(object componentObject, IType type)
        {
            var componentType = type as ComponentType;
            if (componentType != null)
            {
                for (int i = 0; i < componentType.PropertyNames.Length; i++)
                {
                    var propertyName = componentType.PropertyNames[i];
                    if (componentType.Subtypes[i].IsComponentType)
                    {
                        var value = componentObject.GetType().GetProperty(propertyName).GetValue(componentObject, null);
                        NormalizeDateTimePropertiesForComponentType(value, componentType.Subtypes[i]);
                    }

                    if (componentType.Subtypes[i].ReturnedClass != typeof(DateTime) && componentType.Subtypes[i].ReturnedClass != typeof(DateTime?))
                    {
                        continue;
                    }

                    var dateTime = componentObject.GetType().GetProperty(propertyName).GetValue(componentObject, null) as DateTime?;

                    if (!dateTime.HasValue)
                    {
                        continue;
                    }

                    componentObject.GetType().GetProperty(propertyName).SetValue(componentObject, dateTime.Value, null);
                }
            }
        }

        protected virtual void TriggerDomainEvents(object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            var domainEvents = generatesDomainEventsEntity.DomainEvents.ToList();
            generatesDomainEventsEntity.DomainEvents.Clear();

            foreach (var domainEvent in domainEvents)
            {
                _eventBus.Value.Trigger(domainEvent.GetType(), entityAsObj, domainEvent);
            }
        }
    }
}
