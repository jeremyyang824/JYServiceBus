using System;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; }

        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
            this.MemberList = MemberList.Destination;
        }

        public AutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
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
                configuration.CreateMap(targetType, type, MemberList);
            }
        }
    }
}
