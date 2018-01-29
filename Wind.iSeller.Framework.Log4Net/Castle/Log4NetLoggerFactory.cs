using System;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Wind.iSeller.Framework.Log4Net.Castle
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";
        private readonly ILoggerRepository _loggerRepository;

        public Log4NetLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
            var file = GetConfigFile(configFileName);
            XmlConfigurator.ConfigureAndWatch(file);
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new Log4NetLogger(LogManager.GetLogger(name), this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}