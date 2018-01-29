using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.Reflection;

namespace Wind.iSeller.Framework.Core.PlugIns
{
    /// <summary>
    /// 加载指定文件夹下的程序集
    /// </summary>
    public class FolderPlugInSource : IPlugInSource
    {
        public string Folder { get; private set; }

        public SearchOption SearchOption { get; set; }

        private readonly Lazy<List<Assembly>> _assemblies;
        
        public FolderPlugInSource(string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Folder = folder;
            SearchOption = searchOption;

            _assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public List<Assembly> GetAssemblies()
        {
            return _assemblies.Value;
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            var assemblyList = this.GetAssemblies();
            foreach (var assembly in assemblyList)
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
            return AssemblyHelper.GetAllAssembliesInFolder(Folder, SearchOption);
        }
    }
}