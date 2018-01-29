using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wind.iSeller.Framework.Core.Modules
{
    /// <summary>
    /// Used to store all needed information for a module.
    /// </summary>
    public class WindModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public WindModule Instance { get; private set; }

        /// <summary>
        /// Is this module loaded as a plugin.
        /// </summary>
        public bool IsLoadedAsPlugIn { get; private set; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<WindModuleInfo> Dependencies { get; private set; }

        /// <summary>
        /// Creates a new WindModuleInfo object.
        /// </summary>
        public WindModuleInfo(Type type, WindModule instance, bool isLoadedAsPlugIn)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (instance == null)
                throw new ArgumentNullException("instance");

            Type = type;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;
            Assembly = Type.Assembly;

            Dependencies = new List<WindModuleInfo>();
        }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ?? Type.FullName;
        }
    }
}
