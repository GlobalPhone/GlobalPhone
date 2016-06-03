using System;

namespace GlobalPhone
{
    [Serializable]
    public class UnknownRegionException : Exception
    {
        public UnknownRegionException(string regionName)
        {
            Region = regionName;
        }

        public string Region { get; set; }
    }
}
