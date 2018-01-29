using System;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; }

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
            this.MemberList = MemberList.Source;
        }

        public AutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            MemberList = memberList;
        }

        public override void CreateMap(IProfileExpression configuration, Type type)
        {
            if (this.TargetTypes == null || this.TargetTypes.Length < 1)
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateMap(type, targetType, MemberList);
            }
        }
    }
}
