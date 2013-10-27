using System;
using System.Runtime.Serialization;

namespace GlobalPhone
{
    [Serializable]
    public class NoDatabaseException : Exception
    {
        public NoDatabaseException()
        {
        }

        public NoDatabaseException(string message)
            : base(message)
        {
        }

        public NoDatabaseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoDatabaseException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}