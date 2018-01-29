using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.AutoMapper;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.Reflection.Extensions;
using Wind.iSeller.Framework.Log4Net;
using Wind.iSeller.Framework.NHibernate;
using Wind.iSeller.Data.Core;

namespace Wind.iSeller.Data.Test
{
    [DependsOn(
        typeof(WindKernelModule),
        typeof(WindCastleLog4NetModule),
        typeof(WindAutoMapperModule),
        typeof(WindNHibernateModule),
        typeof(ISellerDataCoreModule))]
    public class DataTestModule : WindModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "ISellerConnection";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DataTestModule).GetAssembly());
        }
    }
}
