using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wind.iSeller.NServiceBus.ZeroService.Beans
{
    /// <summary>
    /// 集合类型
    /// </summary>
    public class ServiceMsgCollectionTypeDefine : IServiceMsgTypeDefine
    {
        private IServiceMsgTypeDefine itemElement;

        public ServiceMsgCollectionTypeDefine(IServiceMsgTypeDefine itemElement)
        {
            this.itemElement = itemElement;
        }

        public string GetFormatString()
        {
            return this.GetFormatString(0);
        }

        public string GetFormatString(int ident)
        {
            return string.Format("{0}[]", this.itemElement.GetFormatString(ident));
        }
    }
}
