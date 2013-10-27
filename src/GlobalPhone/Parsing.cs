using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public abstract class Parsing
    {
        public Number parse(string str, string territory_name)
        {
            str = Number.normalize(str);
            var territory = this.territory(territory_name).Unless(new ArgumentException("unknown territory `"+territory_name+"'"));
            if (starts_with_plus(str))
            {
                return parse_international_string(str);
            }
            else if (str.match(territory.international_prefix).Success)
            {
                str = strip_international_prefix(territory, str);
                return parse_international_string(str);
            }else
            {
                return territory.parse_national_string(str);
            }
        }

        private string strip_international_prefix(Territory territory, string @string)
        {
            return @string.gsub(territory.international_prefix, "");
        }

        private Number parse_international_string(string @string)
        {
            @string = Number.normalize(@string);
            if (starts_with_plus(@string))
            {
                @string = strip_leading_plus(@string) ;
            }
            Region region; 
            if ((region = region_for_string(@string))!=null)
            {
                return region.parse_national_string(@string);
            }
            return null;
        }

        private IEnumerable<String> country_code_candidates_for(string @string)
        {
            return new[] {1, 2, 3}.Map(i=>@string.Substring(0,i));
        }

        private string strip_leading_plus(string str)
        {
            return str.Substring(1);
        }

        private bool starts_with_plus(string str)
        {
            return str.StartsWith("+");
        }

        private Region region_for_string(string @string)
        {
            var candidates = country_code_candidates_for(@string);
            return Utils.map_detect(candidates, country_code => region(country_code));
        }

        public abstract Region region(String countryCode);

        public abstract Territory territory(string territoryName);
    }
}