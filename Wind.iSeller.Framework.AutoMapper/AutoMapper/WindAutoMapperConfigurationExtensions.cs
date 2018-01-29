using System;
using Wind.iSeller.Framework.Core.Configuration.Startup;

namespace Wind.iSeller.Framework.AutoMapper
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.AutoMapper module.
    /// </summary>
    public static class WindAutoMapperConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.AutoMapper module.
        /// </summary>
        public static IWindAutoMapperConfiguration WindAutoMapper(this IModuleConfigurations configurations)
        {
            return configurations.WindConfiguration.Get<IWindAutoMapperConfiguration>();
        }
    }
}
