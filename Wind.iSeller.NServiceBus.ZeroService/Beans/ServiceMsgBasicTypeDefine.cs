using System;

namespace Wind.iSeller.NServiceBus.ZeroService.Beans
{
    /// <summary>
    /// 基本类型
    /// </summary>
    public class ServiceMsgBasicTypeDefine : IServiceMsgTypeDefine
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 可空类型
        /// </summary>
        public bool IsNullable { get; set; }

        public ServiceMsgBasicTypeDefine(string typeName, bool isNullable = false)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException("typeName");

            this.TypeName = typeName;
            this.IsNullable = IsNullable;
        }

        public static bool IsBasicType(Type type)
        {
            if (type.IsPrimitive)
                return true;
            if (type == typeof(string))
                return true;
            if (type == typeof(decimal))
                return true;
            return false;
        }

        public string GetFormatString()
        {
            return this.GetFormatString(0);
        }

        public string GetFormatString(int ident)
        {
            if (this.IsNullable)
                return this.TypeName + "?";
            return this.TypeName;
        }
    }
}
