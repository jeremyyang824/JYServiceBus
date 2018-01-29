using System;
using System.Collections.Generic;
using Wind.iSeller.NServiceBus.ZeroService.Beans;

namespace Wind.iSeller.NServiceBus.ZeroService.Domain
{
    public interface IScriptProxyGenerator
    {
        /// <summary>
        /// 生成器名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 生成js字符串
        /// </summary>
        /// <param name="CommandDefineList">命令集合</param>
        /// <returns>js字符串</returns>
        string Build(IEnumerable<ServiceCommandDefine> commandDefineList);
    }
}
