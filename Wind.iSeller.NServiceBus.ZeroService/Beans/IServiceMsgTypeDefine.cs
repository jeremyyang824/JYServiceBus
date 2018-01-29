using System;

namespace Wind.iSeller.NServiceBus.ZeroService.Beans
{
    /// <summary>
    /// 代表一个服务命令/事件的（输入/输出）类型定义
    /// </summary>
    public interface IServiceMsgTypeDefine
    {
        /// <summary>
        /// 取得格式化字符串
        /// </summary>
        string GetFormatString();

        string GetFormatString(int ident);
    }
}
