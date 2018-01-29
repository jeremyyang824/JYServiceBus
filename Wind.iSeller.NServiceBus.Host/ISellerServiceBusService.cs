using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Wind.iSeller.NServiceBus.Host
{
    partial class ISellerServiceBusService : ServiceBase
    {
        private WindServiceBusApplication<WindServiceBusHostModule> app = null;

        public ISellerServiceBusService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            app = new WindServiceBusApplication<WindServiceBusHostModule>();
            app.Start();

            ServiceHostEagle.Initilize();
        }

        protected override void OnStop()
        {
            app.Stop();

            ServiceHostEagle.Dispose();
            Process processSelf = Process.GetCurrentProcess();
            processSelf.Kill();
        }
    }
}
