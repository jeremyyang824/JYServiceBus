using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Domain.Uow
{
    public class DataFilterConfiguration
    {
        public string FilterName { get; private set; }

        public bool IsEnabled { get; private set; }

        public IDictionary<string, object> FilterParameters { get; private set; }

        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            FilterName = filterName;
            IsEnabled = isEnabled;
            FilterParameters = new Dictionary<string, object>();
        }

        internal DataFilterConfiguration(DataFilterConfiguration filterToClone, bool? isEnabled = null)
            : this(filterToClone.FilterName, isEnabled ?? filterToClone.IsEnabled)
        {
            foreach (var filterParameter in filterToClone.FilterParameters)
            {
                FilterParameters[filterParameter.Key] = filterParameter.Value;
            }
        }
    }
}