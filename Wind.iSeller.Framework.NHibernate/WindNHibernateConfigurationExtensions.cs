using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.NHibernate.Configuration;

namespace Wind.iSeller.Framework.NHibernate
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Wind NHibernate module.
    /// </summary>
    public static class WindNHibernateConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Wind NHibernate module.
        /// </summary>
        public static IWindNHibernateModuleConfiguration WindNHibernate(this IModuleConfigurations configurations)
        {
            return configurations.WindConfiguration.Get<IWindNHibernateModuleConfiguration>();
        }
    }
}
