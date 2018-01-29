using System;
using System.Reflection;

namespace Wind.iSeller.Framework.Core.Reflection.Extensions
{
    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }
    }
}
