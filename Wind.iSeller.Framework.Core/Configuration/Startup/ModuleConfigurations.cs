
namespace Wind.iSeller.Framework.Core.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IWindStartupConfiguration WindConfiguration { get; private set; }

        public ModuleConfigurations(IWindStartupConfiguration windConfiguration)
        {
            WindConfiguration = windConfiguration;
        }
    }
}
