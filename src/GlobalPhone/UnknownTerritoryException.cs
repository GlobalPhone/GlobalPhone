using System;

namespace GlobalPhone
{
    /// <summary>
    /// Unknown territory exception.
    /// </summary>
    [Serializable]
    public class UnknownTerritoryException : Exception
    {
        /// <summary>
        /// </summary>
        public UnknownTerritoryException(string territoryName)
        {
            Territory = territoryName;
        }
        /// <summary>
        /// Gets or sets the territory.
        /// </summary>
        /// <value>The territory.</value>
        public string Territory { get; set; }
    }
}