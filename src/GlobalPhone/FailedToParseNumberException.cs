using System;
using System.Runtime.Serialization;

namespace GlobalPhone
{
    /// <summary>
    /// Failed to parse number exception.
    /// </summary>
    public class FailedToParseNumberException : Exception
    {
        /// <summary>
        /// </summary>
        public FailedToParseNumberException()
        {
        }
        /// <summary>
        /// </summary>
        public FailedToParseNumberException(string message) : base(message)
        {
        }
        /// <summary>
        /// </summary>
        public FailedToParseNumberException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
    