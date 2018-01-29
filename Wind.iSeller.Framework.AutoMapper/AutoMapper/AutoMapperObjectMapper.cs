using System;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public class AutoMapperObjectMapper : Wind.iSeller.Framework.Core.ObjectMapping.IObjectMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}
