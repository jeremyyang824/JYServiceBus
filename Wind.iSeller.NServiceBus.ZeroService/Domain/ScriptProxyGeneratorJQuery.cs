using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.ZeroService.Beans;

namespace Wind.iSeller.NServiceBus.ZeroService.Domain
{
    /// <summary>
    /// 生成基于jQuery的脚本
    /// </summary>
    public class ScriptProxyGeneratorJQuery : IScriptProxyGenerator, ITransientDependency
    {
        /// <summary>
        /// 生成器名
        /// </summary>
        public string Name { get { return "jQuery"; } }

        /// <summary>
        /// 生成js字符串
        /// </summary>
        /// <returns></returns>
        public string Build(IEnumerable<ServiceCommandDefine> commandDefineList)
        {
            StringBuilder sBuilder = new StringBuilder();

            var assemblies = commandDefineList.GroupBy(c => c.ServiceAssemblyName);
            foreach (var serviceAssembly in assemblies)
            {
                string assemblyName = serviceAssembly.Key;

                //验证
                if (string.IsNullOrWhiteSpace(assemblyName))
                {
                    continue;
                    //throw new ScriptProxyGeneratorException("assemblyName empty exception!");
                }

                //创建模块闭包
                var module = this.BuildServiceAssemblyModule(assemblyName, serviceAssembly);
                sBuilder.Append(module);
                sBuilder.AppendLine();
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 构建一个模块
        /// </summary>
        /// <param name="assemblyName">服务程序集名称</param>
        /// <param name="commandDefineList">命令定义</param>
        /// <returns>js字符串</returns>
        protected virtual string BuildServiceAssemblyModule(string assemblyName, IEnumerable<ServiceCommandDefine> commandDefineList)
        {
            //过滤重复并排序
            List<ServiceCommandDefine> cmdlist = commandDefineList.Distinct().ToList();
            cmdlist.Sort();

            StringBuilder moduleBuilder = new StringBuilder();

            //模块开始
            moduleBuilder.Append(MODULE_CONTAINER_BEGIN);

            //命名空间
            var moduleNamespace = this.BuildModuleNameSpace(assemblyName);
            moduleBuilder.AppendLine();
            moduleBuilder.AppendLine(moduleNamespace);

            //命令方法
            foreach (ServiceCommandDefine commandDefine in commandDefineList)
            {
                var command = this.BuildServiceCommand(commandDefine);
                moduleBuilder.Append(command);
            }

            //模块结束
            moduleBuilder.Append(MODULE_CONTAINER_END);

            return moduleBuilder.ToString();
        }

        protected virtual string BuildServiceCommand(ServiceCommandDefine commandDefine)
        {
            StringBuilder commandBuilder = new StringBuilder();

            //方法注释
            this.BuildServiceCommandComment(commandBuilder, commandDefine);

            //方法明细
            commandBuilder.AppendLine(string.Format(SERVICE_COMMAND, ROOT_NAMESPACE, commandDefine.ServiceAssemblyName, commandDefine.ServiceCommandName));
            return commandBuilder.ToString();
        }

        protected virtual void BuildServiceCommandComment(StringBuilder commandBuilder, ServiceCommandDefine commandDefine)
        {
            commandBuilder.AppendLine("    /**");
            commandBuilder.AppendLine("     * @input: " + commandDefine.InputTypeDefine.Replace("\r\n", "\r\n     * "));
            commandBuilder.AppendLine("     * ");
            commandBuilder.AppendLine("     * @output: " + commandDefine.OutputTypeDefine.Replace("\r\n", "\r\n     * "));
            commandBuilder.AppendLine("     */");
        }

        protected virtual string BuildModuleNameSpace(string assemblyName)
        {
            StringBuilder nsBuilder = new StringBuilder();
            var moduleSegments = assemblyName.Trim().Split('.');
            var cur = ROOT_NAMESPACE;
            foreach (var segment in moduleSegments)
            {
                cur = string.Format("{0}.{1}", cur, segment);
                nsBuilder.AppendLine(string.Format("    {0} = {0} || {{}};", cur));
            }
            return nsBuilder.ToString();
        }


        #region template

        private const string ROOT_NAMESPACE = "servicebusProxy";

        private const string MODULE_CONTAINER_BEGIN =
@"
(function(servicebusProxy) {
    if (!servicebusProxy) {
        throw 'root object not found!';
    }
";

        private const string MODULE_CONTAINER_END =
@"
})(servicebusProxy);";

        private const string SERVICE_COMMAND =
@"    {0}.{1}.{2} = function(input) {{
        return {0}.ajax.triggerService('{1}', '{2}', input);
    }};
";

        #endregion
    }
}
