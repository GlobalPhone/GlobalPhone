using System;

namespace GlobalPhone
{
    /// <summary>
    /// No database exception.
    /// </summary>
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
    }
}