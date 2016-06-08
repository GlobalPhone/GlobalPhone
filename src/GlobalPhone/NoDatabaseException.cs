using System;
using System.Runtime.Serialization;

namespace GlobalPhone
{
    /// <summary>
    /// No database exception.
    /// </summary>
    [Serializable]
    public class NoDatabaseException : Exception
    {
        /// <summary>
        /// </summary>
        public NoDatabaseException()
        {
        }
        /// <summary>
        /// </summary>
        public NoDatabaseException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// </summary>
        public NoDatabaseException(string message, Exception inner)
            : base(message, inner)
        {
        }
        /// <summary>
        /// </summary>
        protected NoDatabaseException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}