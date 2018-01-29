using System;
using System.Collections.Generic;
using Wind.iSeller.Framework.Core.Events.Bus;

namespace Wind.iSeller.Framework.Core.Domain.Entities
{
    public interface IGeneratesDomainEvents
    {
        ICollection<IEventData> DomainEvents { get; }
    }
}
