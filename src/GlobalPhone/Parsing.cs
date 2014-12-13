using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPhone
{
    public abstract class Parsing
    {
        public Number Parse(string str, string territoryName)
        {
            str = Number.Normalize(str);
            var territory = TryGetTerritory(territoryName).Unless(new UnknownTerritoryException(territoryName));
            if (StartsWithPlus(str))
            {
                return ParseInternationalString(str);
            }
            if (territory.InternationalPrefix.Match(str ?? String.Empty).Success)
            {
                str = StripInternationalPrefix(territory, str);
                return ParseInternationalString(str);
            }
            return territory.ParseNationalString(str);
        }

        private static string StripInternationalPrefix(Territory territory, string @string)
        {
            return territory.InternationalPrefix.Replace(@string, "");
        }

        private Number ParseInternationalString(string @string)
        {
            @string = Number.Normalize(@string);
            if (StartsWithPlus(@string))
            {
                @string = StripLeadingPlus(@string) ;
            }
            Region region; 
            if ((region = RegionForString(@string))!=null)
            {
                return region.ParseNationalString(@string);
            }
            throw new FailedToParseNumberException();
        }

        private static IEnumerable<String> CountryCodeCandidatesFor(string @string)
        {
            return new[] {1, 2, 3}.Select(i => @string.Length <= i ? null : @string.Substring(0, i))
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

        public abstract Region TryGetRegion(String countryCode);

        public abstract Territory TryGetTerritory(string territoryName);
    }
}