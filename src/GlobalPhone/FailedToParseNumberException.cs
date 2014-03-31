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

    [Serializable]
    public class UnknownTerritoryException : Exception
    {
        public UnknownTerritoryException(string territoryName)
        {
            Territory = territoryName;
        }

        public string Territory { get; set; }
    }
}
    