using FluentNHibernate.Cfg;

namespace Wind.iSeller.Framework.NHibernate.Configuration
{
    internal class WindNHibernateModuleConfiguration : IWindNHibernateModuleConfiguration
    {
        public FluentConfiguration FluentConfiguration { get; private set; }

        public WindNHibernateModuleConfiguration()
        {
            FluentConfiguration = Fluently.Configure();
        }
    }
}