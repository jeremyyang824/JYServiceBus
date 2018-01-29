using System;
using System.Collections.Generic;
using System.Linq;

namespace Wind.iSeller.NServiceBus.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new WindServiceBusApplication<WindServiceBusHostModule>();
            app.Start();

            Console.WriteLine("按回车键停止服务...");
            Console.ReadLine();
            app.Stop();
        }
    }
}
