using System;

namespace Wind.iSeller.Framework.Core.Events.Bus.Exceptions
{
    /// <summary>
    /// This type of events are used to notify for exceptions handled by Wind infrastructure.
    /// </summary>
    public class WindHandledExceptionData : ExceptionData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public WindHandledExceptionData(Exception exception)
            : base(exception)
        {

        }
    }
}