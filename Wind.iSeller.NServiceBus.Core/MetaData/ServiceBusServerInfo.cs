using System;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// WindBus配置
    /// </summary>
    public sealed class ServiceBusServerInfo
    {
        /// <summary>
        /// WindBus标识名称
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// Expo服务配置
        /// </summary>
        public ExpoServer ExpoConfig { get; private set; }

        //TODO: 其他协议配置

        public ServiceBusServerInfo(string serverName, ExpoServer expoConfig)
        {
            this.ServerName = serverName;
            this.ExpoConfig = expoConfig;
        }

        public override string ToString()
        {
            return this.ServerName;
        }

        public override int GetHashCode()
        {
            return (string.Format("[{0}]{1}", this.GetType().FullName, this.ServerName)).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ServiceBusServerInfo))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return this.ServerName.Equals(((ServiceBusServerInfo)obj).ServerName);
        }

        /// <summary>
        /// Expo配置
        /// </summary>
        public sealed class ExpoServer
        {
            public int AppClassId { get; private set; }

            public bool IsStart { get; private set; }

            public int CommandId { get; private set; }

            public int Timeout { get; private set; }

            public ExpoServer(int appClassId, int commandId, int timeout, bool isStart)
            {
                this.AppClassId = appClassId;
                this.CommandId = commandId;
                this.IsStart = isStart;
                this.Timeout = timeout;
            }
        }
    }
}
