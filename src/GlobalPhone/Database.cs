using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Makrill;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GlobalPhone
{
    public class Database : Parsing
    {
        public readonly Region[] Regions;

        public Database(object[] recordData)
        {
            _territoriesByName = new Dictionary<string, Territory>(StringComparer.InvariantCultureIgnoreCase);
            Regions = recordData.Map(data => new Region(data)).ToArray();
        }

        public static Database LoadFile(string filename)
        {
            return Load(File.ReadAllText(filename));
        }

        private static Database Load(string text)
        {
            return new Database(Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(text).Map(r1 => JsonConvert.Deserialize((JToken)r1)).ToArray());
        }

        public Region Region(int countryCode)
        {
            return Region(countryCode.ToString(CultureInfo.InvariantCulture));
        }

        public override Region Region(string countryCode)
        {
            Region value;
            return RegionsByCountryCode.TryGetValue(countryCode, out value) ? value : null;
        }

        private Dictionary<string, Region> _regionsByCountryCode;

        protected Dictionary<string, Region> RegionsByCountryCode
        {
            get { return _regionsByCountryCode ?? (_regionsByCountryCode = Regions.ToDictionary(r => r.CountryCode)); }
        }

        private readonly Dictionary<string, Territory> _territoriesByName;
        private static readonly JsonConvert JsonConvert = new JsonConvert();

        public override Territory Territory(string name)
        {
            return _territoriesByName.GetOrAdd(name, () =>
                {
                    Region region;
                    if ((region = RegionForTerritory(name)) != null)
                    {
                        return region.Territory(name);
                    }
                    return null;
                });
        }

        protected Region RegionForTerritory(string name)
        {
            return Regions.SingleOrDefault(r => r.HasTerritory(name));
        }
    }
}