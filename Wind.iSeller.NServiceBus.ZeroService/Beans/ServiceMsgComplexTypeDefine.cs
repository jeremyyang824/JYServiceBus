using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wind.iSeller.NServiceBus.ZeroService.Beans
{
    /// <summary>
    /// 复杂类型
    /// </summary>
    public class ServiceMsgComplexTypeDefine : IServiceMsgTypeDefine, IEnumerable<KeyValuePair<string, IServiceMsgTypeDefine>>
    {
        private Dictionary<string, IServiceMsgTypeDefine> properities
            = new Dictionary<string, IServiceMsgTypeDefine>();

        private int identScale = 2;

        /// <summary>
        /// 添加成员属性
        /// </summary>
        public bool AddProperty(string propName, IServiceMsgTypeDefine propType)
        {
            if (string.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("propName");
            if (propType == null)
                throw new ArgumentNullException("propType");

            if (!properities.ContainsKey(propName))
            {
                properities.Add(propName, propType);
                return true;
            }
            return false;
        }

        public string GetFormatString()
        {
            return this.GetFormatString(0);
        }

        public string GetFormatString(int ident)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.AppendLine("{");

            int idx = 0;
            foreach (var prop in this.properities)
            {
                sbuilder.Append(' ', (ident + 1) * identScale);
                sbuilder.AppendFormat("{0}: {1}", prop.Key, prop.Value.GetFormatString(ident + 1));
                if (idx < this.properities.Count - 1)
                {
                    sbuilder.Append(",");
                }
                sbuilder.AppendLine();
                idx++;
            }

            sbuilder.Append(' ', ident * identScale);
            sbuilder.Append("}");
            return sbuilder.ToString();
        }

        public IEnumerator<KeyValuePair<string, IServiceMsgTypeDefine>> GetEnumerator()
        {
            return this.properities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.properities.GetEnumerator();
        }
    }
}
