using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Domain.Uow
{
    public class ConnectionStringResolveArgs : Dictionary<string, object>
    {
        public static readonly ConnectionStringResolveArgs Empty = new ConnectionStringResolveArgs();

        public ConnectionStringResolveArgs()
        {
        }
    }
}