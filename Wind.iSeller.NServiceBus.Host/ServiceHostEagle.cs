using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using Wind.EAGLE.Log4WCSharp;

namespace Wind.iSeller.NServiceBus.Host
{
    /// <summary>
    /// ServiceHostEagle
    /// </summary>
    public class ServiceHostEagle
    {
        /// <summary>
        /// _Logger
        /// </summary>
        private static ILog _logger = LogManager.GetLogger(typeof(ServiceHostEagle));


        /// <summary>
        /// VMUserdMemory
        /// </summary>
        public static uint VMUserdMemory = 0;

        /// <summary>
        /// TotalRequests
        /// </summary>
        public static uint TotalRequests = 0;

        /// <summary>
        /// AvgRequests
        /// </summary>
        public static uint AvgRequests = 0;

        /// <summary>
        /// _yyyymmdd
        /// </summary>
        private static int _yyyymmdd = GetLongTime(DateTime.Now);

        /// <summary>
        /// _yyyymmddhhmm
        /// </summary>
        private static int _yyyymmddhhmm = GetLongTimeEx(DateTime.Now);

        /// <summary>
        /// Initializes a new instance of the ServiceHostEagle class
        /// </summary>
        public ServiceHostEagle()
        {
        }

        /// <summary>
        /// EagleTrace
        /// </summary>
        public static void EagleTrace()
        {
            TotalRequests++;
            AvgRequests++;
        }

        /// <summary>
        /// yyyymmdd
        /// </summary>
        /// <param name="dt">dt</param>
        /// <returns>int</returns>
        private static int GetLongTime(DateTime dt)
        {
            return (dt.Year * 10000) + (dt.Month * 100) + dt.Day;
        }

        /// <summary>
        /// yyyymmddhhmm
        /// </summary>
        /// <param name="dt">dt</param>
        /// <returns>int</returns>
        private static int GetLongTimeEx(DateTime dt)
        {
            return (dt.Year * 100000000) + (dt.Month * 1000000) +
                (dt.Day * 10000) + (dt.Hour * 100) + dt.Minute;
        }

        /// <summary>
        /// LogInfo
        /// </summary>
        /// <param name="s">s</param>
        public static void LogInfo(string s)
        {
            _logger.Info(s);
        }

        /// <summary>
        /// PerfDataCallBack
        /// </summary>
        /// <param name="processInfo">processInfo</param>
        public static void PerfDataCallBack(ref ProcessInfo processInfo)
        {
            processInfo.VMUsedMemory = (uint)Process.GetCurrentProcess().WorkingSet64;
            processInfo.TotalRequest = TotalRequests;
            processInfo.AvgRequest = AvgRequests;

            int nowtime = GetLongTime(DateTime.Now);
            //clear total requests per day
            if (nowtime > _yyyymmdd)
            {
                _yyyymmdd = nowtime;
                TotalRequests = 0;
            }
            int ic = GetLongTimeEx(DateTime.Now);
            //clear avg per minute
            if (ic != _yyyymmddhhmm)
            {
                _yyyymmddhhmm = ic;
                AvgRequests = 0;
            }

            processInfo.TotalTraffic = uint.MaxValue;
            processInfo.AvgTraffic = uint.MaxValue;
            processInfo.Reserve01 = uint.MaxValue;

            processInfo.Reserve02 = uint.MaxValue;
            processInfo.Reserve03 = uint.MaxValue;
            processInfo.Reserve04 = uint.MaxValue;
            processInfo.Reserve05 = uint.MaxValue;
            processInfo.Reserve06 = uint.MaxValue;
            processInfo.Reserve07 = uint.MaxValue;
            processInfo.Reserve08 = uint.MaxValue;
            processInfo.Reserve09 = uint.MaxValue;
            processInfo.Reserve10 = uint.MaxValue;

            //Marshal.StructureToPtr(info, processInfo, true);
        }

        /// <summary>
        /// ConvertPerfDataName
        /// </summary>
        /// <param name="defaultName">defaultName</param>
        /// <returns>string</returns>
        public static string ConvertPerfDataName(string defaultName)
        {
            if (defaultName == "Reserve01")
            {
                return "HostMemorySize";
            }
            else if (defaultName == "Reserve02")
            {
                return "C#Reserve02";
            }
            else if (defaultName == "Reserve03")
            {
                return "C#Reserve03";
            }
            else if (defaultName == "Reserve04")
            {
                return "C#Reserve04";
            }
            else if (defaultName == "Reserve05")
            {
                return "C#Reserve05";
            }
            else if (defaultName == "Reserve06")
            {
                return "C#Reserve06";
            }
            else if (defaultName == "Reserve07")
            {
                return "C#Reserve07";
            }
            else if (defaultName == "Reserve08")
            {
                return "C#Reserve08";
            }
            else if (defaultName == "Reserve09")
            {
                return "C#Reserve09";
            }
            else if (defaultName == "Reserve10")
            {
                return "C#Reserve10";
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 必须声明为全局静态变量
        /// </summary>
        private static CallBackHandler handler;

        /// <summary>
        /// convertHandler
        /// </summary>
        private static ConvertPerfDataNameHandler convertHandler;

        /// <summary>
        /// Initilize
        /// </summary>
        public static void Initilize()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            convertHandler = new ConvertPerfDataNameHandler(ConvertPerfDataName);
            handler = new CallBackHandler(PerfDataCallBack);

            Log4WWrapper.Initialize(string.Empty);
            Log4WWrapper.RegisterPerfDataNameCallback(convertHandler);
            Log4WWrapper.RegisterCallback(handler);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public static void Dispose()
        {
            //Log4WWrapper.UnInitialize();
        }

        /// <summary>
        /// CurrentDomain_UnhandledException
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">异常</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = string.Empty;
            Exception error = e.ExceptionObject as Exception;
            if (error != null)
            {
                str = string.Format("Application UnhandledExcepiont:{0}\n堆栈信息{1}\n", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}\n", e);
            }
            LogInfo(str);
        }
    }
}
