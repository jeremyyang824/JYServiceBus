using System;
using System.Runtime.Serialization;

namespace Wind.iSeller.Framework.Core.Domain.Entities
{
    [Serializable]
    public class EntityNotFoundException : WindException
    {
        /// <summary>
        /// Type of the entity.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Id of the Entity.
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        public EntityNotFoundException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        public EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        public EntityNotFoundException(Type entityType, object id)
            : this(entityType, id, null)
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        public EntityNotFoundException(Type entityType, object id, Exception innerException)
            : base(string.Format("There is no such an entity. Entity type: {0}, id: {1}", entityType.FullName, id), innerException)
        {
            EntityType = entityType;
            Id = id;
        }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public EntityNotFoundException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
