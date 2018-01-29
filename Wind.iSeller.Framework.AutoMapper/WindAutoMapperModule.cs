using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.Reflection;

namespace Wind.iSeller.Framework.AutoMapper
{
    [DependsOn(typeof(WindKernelModule))]
    public class WindAutoMapperModule : WindModule
    {
        private readonly ITypeFinder _typeFinder;

        private static volatile bool _createdMappingsBefore;
        private static readonly object SyncObj = new object();

        public WindAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IWindAutoMapperConfiguration, WindAutoMapperConfiguration>();

            Configuration.ReplaceService<Wind.iSeller.Framework.Core.ObjectMapping.IObjectMapper, AutoMapperObjectMapper>();

            Configuration.Modules.WindAutoMapper().Configurators.Add(CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }

        private void CreateMappings()
        {
            lock (SyncObj)
            {
                Action<IConfiguration> configurer = configuration =>
                {
                    FindAndAutoMapTypes(configuration);
                    foreach (var configurator in Configuration.Modules.WindAutoMapper().Configurators)
                    {
                        configurator(configuration);
                    }
                };


                if (!_createdMappingsBefore)
                {
                    Mapper.Initialize(configurer);
                    _createdMappingsBefore = true;
                }
            }
        }

        private void FindAndAutoMapTypes(IConfiguration configuration)
        {
            var types = _typeFinder.Find(type =>
            {
                var typeInfo = type;
                return typeInfo.IsDefined(typeof(AutoMapAttribute), false) ||
                       typeInfo.IsDefined(typeof(AutoMapFromAttribute), false) ||
                       typeInfo.IsDefined(typeof(AutoMapToAttribute), false);
            });

            Logger.DebugFormat("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IConfiguration configuration)
        {

        }
    }
}
