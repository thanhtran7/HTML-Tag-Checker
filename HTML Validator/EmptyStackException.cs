using System;
using System.Runtime.Serialization;

namespace lab4b
{
    [Serializable]
    internal class EmptyStackException : Exception
    {
        public EmptyStackException()
        {
        }

        public EmptyStackException(string message) : base(message)
        {
        }

        public EmptyStackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyStackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}