using System;
using System.Runtime.Serialization;

namespace lab4b
{
    [Serializable]
    internal class FullStackException : Exception
    {
        public FullStackException()
        {
        }

        public FullStackException(string message) : base(message)
        {
        }

        public FullStackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FullStackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}