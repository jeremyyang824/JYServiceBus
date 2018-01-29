using System;
using System.Configuration;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.Framework.Core.Domain.Uow
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransientDependency
    {
        private readonly IWindStartupConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConnectionStringResolver"/> class.
        /// </summary>
        public DefaultConnectionStringResolver(IWindStartupConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("arg");

            var defaultConnectionString = _configuration.DefaultNameOrConnectionString;
            if (!string.IsNullOrWhiteSpace(defaultConnectionString))
            {
                return defaultConnectionString;
            }
            if (ConfigurationManager.ConnectionStrings["Default"] != null)
            {
                return "Default";
            }

            if (ConfigurationManager.ConnectionStrings.Count == 1)
            {
                return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }

            throw new WindException("Could not find a connection string definition for the application. Set IAbpStartupConfiguration.DefaultNameOrConnectionString or add a 'Default' connection string to application .config file.");
        }
    }
}
