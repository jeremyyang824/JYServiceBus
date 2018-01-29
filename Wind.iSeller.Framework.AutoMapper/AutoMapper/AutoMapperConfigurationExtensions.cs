using System;
using System.Reflection;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    internal static class AutoMapperConfigurationExtensions
    {
        public static void CreateAutoAttributeMaps(this IConfiguration configuration, Type type)
        {
            foreach (var autoMapAttribute in type.GetCustomAttributes(typeof(AutoMapAttributeBase), false))
            {
                ((AutoMapAttributeBase)autoMapAttribute).CreateMap(configuration, type);
            }
        }
    }
}
