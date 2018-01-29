using System;
using System.Diagnostics;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.Framework.Core
{
    /// <summary>
    /// 性能计数
    /// </summary>
    public sealed class WindPerformanceCounter : ISingletonDependency
    {
        /// <summary>
        /// 是否计入日志
        /// </summary>
        public bool IsLogWarnning { get; private set; }

        /// <summary>
        /// 默认超过该阈值将作为警告
        /// </summary>
        public long DefaultWarnningThreshold { get; set; }

        public ILogger Logger { get; set; }

        public WindPerformanceCounter()
        {
            this.IsLogWarnning = true;

            this.DefaultWarnningThreshold = 5 * 1000;   //default 5s
            var configWarnningThreshold = System.Configuration.ConfigurationManager.AppSettings["WindPerformanceCounter.DefaultWarnningThreshold"];
            if (configWarnningThreshold != null)
            {
                this.DefaultWarnningThreshold = long.Parse(configWarnningThreshold);
            }

            Logger = NullLogger.Instance;
        }

        public StopwatchStoper BeginStopwatch(string name, long warnningThreshold = -1)
        {
            if (warnningThreshold < 0)
            {
                warnningThreshold = DefaultWarnningThreshold;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var stoper = new StopwatchStoper(stopWatch, name, warnningThreshold, this.Logger);
            return stoper;
        }

        public class StopwatchStoper : IDisposable
        {
            private readonly string name;
            private readonly Stopwatch stopWatch;
            private readonly long warnningThreshold;
            private readonly ILogger logger;

            /// <summary>
            /// 总运行时间（毫秒）
            /// </summary>
            public long ElapsedMilliseconds { get; private set; }

            public StopwatchStoper(Stopwatch stopWatch, string name, long warnningThreshold, ILogger logger)
            {
                this.name = name;
                this.stopWatch = stopWatch;
                this.warnningThreshold = warnningThreshold;
                this.logger = logger;
            }

            public void Dispose()
            {
                this.stopWatch.Stop();

                this.ElapsedMilliseconds = this.stopWatch.ElapsedMilliseconds;
                if (this.ElapsedMilliseconds > this.warnningThreshold)
                {
                    //超过指定阈值
                    this.logger.WarnFormat("Timeout Warning, Elapsed: [{0}ms], About: [{1}].", this.ElapsedMilliseconds, this.name);
                }
            }
        }
    }
}
