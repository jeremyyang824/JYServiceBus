using System;
using System.Collections.Generic;
using System.Reflection;
using Wind.Core.NHibernate.Uow;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.NHibernate.Configuration;
using Wind.iSeller.Framework.NHibernate.Interceptors;
using Wind.iSeller.Framework.NHibernate.Repositories;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using NHibernate;

namespace Wind.iSeller.Framework.NHibernate
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    [DependsOn(typeof(WindKernelModule))]
    public class WindNHibernateModule : WindModule
    {
        /// <summary>
        /// NHibernate session factory object.
        /// </summary>
        private ISessionFactory _sessionFactory;

        public override void PreInitialize()
        {
            IocManager.Register<IWindNHibernateModuleConfiguration, WindNHibernateModuleConfiguration>();
            Configuration.ReplaceService<IUnitOfWorkFilterExecuter, NhUnitOfWorkFilterExecuter>(DependencyLifeStyle.Transient);
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.Register<WindNHibernateInterceptor>(DependencyLifeStyle.Transient);

            _sessionFactory = Configuration.Modules.WindNHibernate().FluentConfiguration
                //.Mappings(m => m.FluentMappings.Add(typeof(MayHaveTenantFilter)))
                //.Mappings(m => m.FluentMappings.Add(typeof(MustHaveTenantFilter)))
                .ExposeConfiguration(config => config.SetInterceptor(IocManager.Resolve<WindNHibernateInterceptor>()))
                .BuildSessionFactory();

            IocManager.IocContainer.Install(new NhRepositoryInstaller(_sessionFactory));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        /// <inheritdoc/>
        public override void Shutdown()
        {
            _sessionFactory.Dispose();
        }
    }
}
