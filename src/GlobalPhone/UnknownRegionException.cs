using System;

namespace GlobalPhone
{
    /// <summary>
    /// Unknown region exception.
    /// </summary>
    public class UnknownRegionException : Exception
    {
        /// <summary>
        /// </summary>
        public UnknownRegionException(string regionName)
        {
            Region = regionName;
        }
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public string Region { get; set; }
    }
}
