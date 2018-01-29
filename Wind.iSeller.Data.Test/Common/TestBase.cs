using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Castle.Facilities.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.PlugIns;
using Wind.iSeller.Framework.Log4Net.Castle;

namespace Wind.iSeller.Data.Test.Common
{
    public abstract class TestBase<TStartupModule> : IDisposable
        where TStartupModule : WindModule
    {
        /// <summary>
        /// Local <see cref="IIocManager"/> used for this test.
        /// </summary>
        protected IIocManager LocalIocManager { get; private set; }

        protected IUnitOfWorkManager UnitOfWorkManager { get; private set; }

        protected WindBootstrapper WindBootstrapper { get; private set; }

        protected TestBase(bool initialize = true)
        {
            this.LocalIocManager = new IocManager();
            this.WindBootstrapper = WindBootstrapper.Create<TStartupModule>(LocalIocManager);

            this.WindBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseWindLog4Net().WithConfig("log4net.config")
            );

            if (initialize)
            {
                Initialize();
            }
        }

        private const long ElapseLimited = 500; //500ms限制
        private Stopwatch stopwatch;
        private IUnitOfWorkCompleteHandle uowHandler;

        /// <summary>
        /// 开始某个测试用例
        /// </summary>
        [TestInitialize]
        public void MethodInitialize()
        {
            stopwatch = Stopwatch.StartNew();
            uowHandler = UnitOfWorkManager.Begin();
        }

        /// <summary>
        /// 结束某个测试用例
        /// </summary>
        [TestCleanup]
        public void MethodCleanup()
        {
            uowHandler.Complete();
            uowHandler.Dispose();

            long elapsed = stopwatch.ElapsedMilliseconds;
            //Assert.IsTrue(elapsed < ElapseLimited, string.Format("超时，实际运行时间: {0}ms", elapsed));
        }

        #region LifeCycle

        protected void Initialize()
        {
            PreInitialize();

            WindBootstrapper.Initialize();

            PostInitialize();
        }

        protected virtual void PreInitialize()
        {

        }

        protected virtual void PostInitialize()
        {
            this.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        public virtual void Dispose()
        {
            WindBootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        #endregion

        #region Resolve

        protected T Resolve<T>()
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        protected object Resolve(Type type)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type);
        }

        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }

        protected void EnsureClassRegistered(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                if (!type.IsClass || type.IsAbstract)
                {
                    throw new WindException("Can not register " + type.Name + ". It should be a non-abstract class. If not, it should be registered before.");
                }
                LocalIocManager.Register(type, lifeStyle);
            }
        }

        #endregion
    }
}
