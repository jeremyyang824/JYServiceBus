using System;
using System.Linq;
using System.Reflection;
using Wind.iSeller.Framework.Core.Dependency;
using Castle.Core;
using Castle.MicroKernel;

namespace Wind.iSeller.Framework.Core.Domain.Uow
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for Unit Of Work mechanism.
    /// </summary>
    internal static class UnitOfWorkRegistrar
    {
        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="iocManager">IOC manager</param>
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation;

                HandleTypesWithUnitOfWorkAttribute(implementationType, handler);
                HandleConventionalUnitOfWorkTypes(iocManager, implementationType, handler);
            };
        }

        private static void HandleTypesWithUnitOfWorkAttribute(Type implementationType, IHandler handler)
        {
            if (IsUnitOfWorkType(implementationType) || AnyMethodHasUnitOfWork(implementationType))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }

        private static void HandleConventionalUnitOfWorkTypes(IIocManager iocManager, Type implementationType, IHandler handler)
        {
            if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>())
            {
                return;
            }

            var uowOptions = iocManager.Resolve<IUnitOfWorkDefaultOptions>();

            if (uowOptions.IsConventionalUowClass(implementationType))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }

        private static bool IsUnitOfWorkType(Type implementationType)
        {
            return UnitOfWorkHelper.HasUnitOfWorkAttribute(implementationType);
        }

        private static bool AnyMethodHasUnitOfWork(Type implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(UnitOfWorkHelper.HasUnitOfWorkAttribute);
        }
    }
}
