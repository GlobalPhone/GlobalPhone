using System;

namespace GlobalPhone
{
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