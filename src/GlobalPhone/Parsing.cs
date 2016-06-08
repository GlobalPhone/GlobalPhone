using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPhone
{
    public abstract class Parsing
    {
        public Number Parse(string str, string territoryName)
        {
            var territory = GetTerritory(territoryName);
            str = territory.Normalize(str);

            if (StartsWithPlus(str))
            {
                return ParseInternationalString(str);
            }
            string stripped;
            if (territory.Region.TryStripInternationalPrefix(str, out stripped))
            {
                return ParseInternationalString(stripped);
            }
            return territory.ParseNationalString(str);
        }

        private Number ParseInternationalString(string @string)
        {
            @string = Number.Normalize(@string, null);
            if (StartsWithPlus(@string))
            {
                @string = StripLeadingPlus(@string);
            }
            Region region;
            if ((region = RegionForString(@string)) != null)
            {
                return region.ParseNationalString(@string);
            }
            throw new FailedToParseNumberException();
        }

        private static IEnumerable<String> CountryCodeCandidatesFor(string @string)
        {
            return new[] { 1, 2, 3 }.Select(i => @string.Length <= i ? null : @string.Substring(0, i))
                .Where(candidate => !String.IsNullOrEmpty(candidate));
        }

        private static string StripLeadingPlus(string str)
        {
            return str.Substring(1);
        }

        private bool StartsWithPlus(string str)
        {
            return str.StartsWith("+");
        }

        private Region RegionForString(string @string)
        {
            var candidates = CountryCodeCandidatesFor(@string);
            return candidates.SelectWhereNotNull(TryGetRegion);
        }

        public abstract bool TryGetRegion(string countryCode, out Region region);

        public abstract bool TryGetTerritory(string territoryName, out Territory territory);

        public Territory TryGetTerritory(string territoryName)
        {
            Territory territory;
            if (!TryGetTerritory(territoryName, out territory))
            {
                return null;
            }
            return territory;
        }

        public Territory GetTerritory(string territoryName)
        {
            Territory territory;
            if (!TryGetTerritory(territoryName, out territory))
            {
                throw new UnknownTerritoryException(territoryName);
            }
            return territory;
        }

        public Region GetRegion(string regionName)
        {
            Region region;
            if (!TryGetRegion(regionName, out region))
            {
                throw new UnknownRegionException(regionName);
            }
            return region;
        }

        public Region TryGetRegion(string regionName)
        {
            Region region;
            if (!TryGetRegion(regionName, out region))
            {
                return null;
            }
            return region;
        }
    }
}