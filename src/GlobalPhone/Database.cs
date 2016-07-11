using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GlobalPhone
{
    /// <summary>
    /// Database of phone number information.
    /// </summary>
    public class Database : Parsing
    {
        public readonly Region[] Regions;

        public Database(object[] recordData)
        {
            _territoriesByName = new Dictionary<string, Territory>(StringComparer.OrdinalIgnoreCase);
            Regions = recordData.Select(data => new Region(data)).ToArray();
        }

        public static Database LoadFile(string filename, IDeserializer serializer)
        {
            return Load(File.ReadAllText(filename), serializer);
        }

        public static Database Load(string text, IDeserializer serializer)
        {
            return new Database(serializer.Deserialize(text));
        }

        public bool TryGetRegion(int countryCode, out Region value)
        {
            return TryGetRegion(countryCode.ToString(CultureInfo.InvariantCulture), out value);
        }

        public Region TryGetRegion(int countryCode)
        {
            Region value;
            return TryGetRegion(countryCode.ToString(CultureInfo.InvariantCulture), out value) ? value : null;
        }

        public override bool TryGetRegion(string countryCode, out Region value)
        {
            return RegionsByCountryCode.TryGetValue(countryCode, out value);
        }

        private Dictionary<string, Region> _regionsByCountryCode;

        protected Dictionary<string, Region> RegionsByCountryCode
        {
            get { return _regionsByCountryCode ?? (_regionsByCountryCode = Regions.ToDictionary(r => r.CountryCode)); }
        }

        private readonly Dictionary<string, Territory> _territoriesByName;

        public override bool TryGetTerritory(string name, out Territory territory)
        {
            Territory value;
            if (_territoriesByName.TryGetValue(name, out value))
            {
                territory = value;
                return true;
            }

            Region region;
            if ((region = RegionForTerritory(name)) != null
                && (territory = region.Territory(name)) != null)
            {
                _territoriesByName.Add(name, territory);
                return true;
            }
            territory = null;
            return false;
        }

        private Region RegionForTerritory(string name)
        {
            return Regions.SingleOrDefault(r => r.HasTerritory(name));
        }
    }
}