using System;

namespace Wind.iSeller.Framework.Core.Events.Bus.Entities
{
    [Serializable]
    public class DomainEventEntry
    {
        public object SourceEntity { get; private set; }

        public IEventData EventData { get; private set; }

        public DomainEventEntry(object sourceEntity, IEventData eventData)
        {
            SourceEntity = sourceEntity;
            EventData = eventData;
        }
    }
}
