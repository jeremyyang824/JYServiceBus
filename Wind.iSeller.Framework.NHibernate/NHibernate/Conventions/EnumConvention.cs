using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Wind.iSeller.Framework.NHibernate.NHibernate.Conventions
{
    /// <summary>
    /// 枚举转int
    /// </summary>
    public class EnumConvention : IUserTypeConvention
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType(instance.Property.PropertyType);
        }

        public bool Accept(Type type)
        {
            return type.IsEnum;
        }
    }
}
