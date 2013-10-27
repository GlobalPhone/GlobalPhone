using System;
using System.Runtime.Serialization;

namespace GlobalPhone
{
    [Serializable]
    public class NoDatabaseException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

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