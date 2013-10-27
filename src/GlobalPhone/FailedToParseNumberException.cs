using System;
using System.Runtime.Serialization;

namespace GlobalPhone
{
    [Serializable]
    public class FailedToParseNumberException : Exception
    {
        public FailedToParseNumberException()
        {
        }

        public FailedToParseNumberException(string message) : base(message)
        {
        }

        public FailedToParseNumberException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FailedToParseNumberException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
