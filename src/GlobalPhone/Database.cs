using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
#if NEWTONSOFT
using Makrill;
using Newtonsoft.Json.Linq;
#else
using System.Web.Script.Serialization;
#endif
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

        public static Database Load(string text)
        {
#if NEWTONSOFT
            return new Database(JArray.Parse(text).Map(r1 => JsonConvert.Deserialize(r1)).ToArray());
#else
            return new Database(JsonConvert.Deserialize<object[]>(text));
#endif
        }

        public Region TryGetRegion(int countryCode)
        {
            return TryGetRegion(countryCode.ToString(CultureInfo.InvariantCulture));
        }

        public override Region TryGetRegion(string countryCode)
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
#if NEWTONSOFT
        private static readonly JsonConvert JsonConvert = new JsonConvert();
#else
        private static readonly JavaScriptSerializer JsonConvert = new JavaScriptSerializer();
#endif
      
        public override Territory TryGetTerritory(string name)
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