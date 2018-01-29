using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Data
{
    public class ActiveTransactionProviderArgs : Dictionary<string, object>
    {
        public static ActiveTransactionProviderArgs Empty { get; private set; }

        static ActiveTransactionProviderArgs()
        {
            Empty = new ActiveTransactionProviderArgs();
        }
    }
}
