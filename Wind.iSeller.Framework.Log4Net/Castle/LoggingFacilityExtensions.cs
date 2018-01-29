using Castle.Facilities.Logging;

namespace Wind.iSeller.Framework.Log4Net.Castle
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseWindLog4Net(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<Log4NetLoggerFactory>();
        }
    }
}