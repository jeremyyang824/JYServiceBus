using System;
using System.Runtime.Serialization;

namespace Wind.iSeller.Framework.Core
{
    [Serializable]
    public class WindException : Exception
    {
        public WindException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="WindException"/> object.
        /// </summary>
        public WindException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        public WindException(string message)
            : base(message)
        {

        }

        public WindException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
