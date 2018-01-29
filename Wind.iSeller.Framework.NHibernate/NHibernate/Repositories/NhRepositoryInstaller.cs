using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Wind.iSeller.Framework.Core.Domain.Repositories;

namespace Wind.iSeller.Framework.NHibernate.Repositories
{
    internal class NhRepositoryInstaller : IWindsorInstaller
    {
        private readonly ISessionFactory _sessionFactory;

        public NhRepositoryInstaller(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ISessionFactory>().UsingFactoryMethod(() => _sessionFactory).LifeStyle.Singleton,
                Component.For(typeof (IRepository<,>)).ImplementedBy(typeof (NhRepositoryBase<,>)).LifestyleTransient()
                );
        }
    }
}
