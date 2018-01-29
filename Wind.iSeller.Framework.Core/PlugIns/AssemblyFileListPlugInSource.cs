using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Wind.iSeller.Framework.Core.Modules;

namespace Wind.iSeller.Framework.Core.PlugIns
{
    /// <summary>
    /// 加载指定路径的程序集
    /// </summary>
    public class AssemblyFileListPlugInSource : IPlugInSource
    {
        public string[] FilePaths { get; private set; }

        private readonly Lazy<List<Assembly>> _assemblies;

        public AssemblyFileListPlugInSource(params string[] filePaths)
        {
            FilePaths = filePaths ?? new string[0];

            _assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public List<Assembly> GetAssemblies()
        {
            return _assemblies.Value;
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (WindModule.IsWindModule(type))
                        {
                            modules.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new WindException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules;
        }

        private List<Assembly> LoadAssemblies()
        {
            return FilePaths.Select(Assembly.LoadFile).ToList();
        }
    }
}