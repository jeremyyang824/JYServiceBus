using System;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Modules;

namespace Wind.iSeller.Framework.Log4Net
{
    [DependsOn(typeof(WindKernelModule))]
    public class WindCastleLog4NetModule : WindModule
    {
    }
}
