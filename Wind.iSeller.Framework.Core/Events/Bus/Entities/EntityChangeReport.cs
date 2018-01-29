using System;
using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Events.Bus.Entities
{
    public class EntityChangeReport
    {
        public List<EntityChangeEntry> ChangedEntities { get; private set; }

        public List<DomainEventEntry> DomainEvents { get; private set; }

        public EntityChangeReport()
        {
            ChangedEntities = new List<EntityChangeEntry>();
            DomainEvents = new List<DomainEventEntry>();
        }

        public bool IsEmpty()
        {
            return ChangedEntities.Count <= 0 && DomainEvents.Count <= 0;
        }

        public override string ToString()
        {
            return string.Format("[EntityChangeReport] ChangedEntities: {0}, DomainEvents: {1}", ChangedEntities.Count, DomainEvents.Count);
        }
    }
}
