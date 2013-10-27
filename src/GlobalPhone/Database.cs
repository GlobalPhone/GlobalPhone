using System;
using System.IO;
using System.Linq;
using Makrill;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GlobalPhone
{
    public class Database : Parsing
    {
        public readonly Region[] regions;

        public Database(object[] recordData)
        {
            territories_by_name = new Dictionary<string, Territory>(StringComparer.InvariantCultureIgnoreCase);
            regions = recordData.Map(data => new Region(data)).ToArray();
        }

        public static Database load_file(string filename)
        {
            return load(File.ReadAllText(filename));
        }

        private static Database load(string text)
        {
            return new Database(Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(text).Map(r1 => _jsonConvert.Deserialize((JToken)r1)).ToArray());
        }

        public Region region(int country_code)
        {
            return region(country_code.ToString());
        }

        public override Region region(string country_code)
        {
            Region value;
            return regions_by_country_code().TryGetValue(country_code, out value) ? value : null;
        }

        private Dictionary<string, Region> _regions_by_country_code;
        protected Dictionary<string, Region> regions_by_country_code()
        {
            return _regions_by_country_code ?? (_regions_by_country_code = regions.ToDictionary(r => r.country_code));
        }

        private Dictionary<string, Territory> territories_by_name;
        private static JsonConvert _jsonConvert = new JsonConvert();

        public override Territory territory(string name)
        {
            return territories_by_name.GetOrAdd(name, () =>
                {
                    Region region = null;
                    if ((region = region_for_territory(name)) != null)
                    {
                        return region.territory(name);
                    }
                    return null;
                });
        }

        protected Region region_for_territory(string name)
        {
            return regions.SingleOrDefault(r => r.has_territory(name));
        }
    }
}