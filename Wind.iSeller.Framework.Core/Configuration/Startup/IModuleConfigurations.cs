namespace Wind.iSeller.Framework.Core.Configuration.Startup
{
    /// <summary>
    /// Used to provide a way to configure modules.
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        /// Gets the Wind configuration object.
        /// </summary>
        IWindStartupConfiguration WindConfiguration { get; }
    }
}

