using System;
using System.Collections.Generic;

namespace GlobalPhone
{
    public abstract class Parsing
    {
        public Number Parse(string str, string territoryName)
        {
            str = Number.Normalize(str);
            var territory = Territory(territoryName).Unless(new ArgumentException("unknown territory `"+territoryName+"'"));
            if (StartsWithPlus(str))
            {
                return ParseInternationalString(str);
            }
            if (str.Match(territory.InternationalPrefix).Success)
            {
                str = StripInternationalPrefix(territory, str);
                return ParseInternationalString(str);
            }
            return territory.parse_national_string(str);
        }

        private static string StripInternationalPrefix(Territory territory, string @string)
        {
            return @string.Gsub(territory.InternationalPrefix, "");
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
                return region.parse_national_string(@string);
            }
            return null;
        }

        private static IEnumerable<String> CountryCodeCandidatesFor(string @string)
        {
            return new[] {1, 2, 3}.Map(i=>@string.Substring(0,i));
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
            return candidates.MapDetect(Region);
        }

        public abstract Region Region(String countryCode);

        public abstract Territory Territory(string territoryName);
    }
}