using System;
using System.Runtime.Serialization;

namespace HyperVRemote.Source.Implementation
{
    [Serializable]
    internal class HyperVException : Exception
    {
        public HyperVException()
        {
        }

        public HyperVException(string message) : base(message)
        {
        }

        public HyperVException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HyperVException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}