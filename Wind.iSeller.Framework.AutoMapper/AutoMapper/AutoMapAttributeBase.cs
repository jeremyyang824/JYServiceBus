using System;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public abstract class AutoMapAttributeBase : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        protected AutoMapAttributeBase(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        public abstract void CreateMap(IProfileExpression configuration, Type type);
    }
}
