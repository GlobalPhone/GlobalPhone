using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Region : Record
    {
        public string country_code;
        private object[] format_record_data;
        private object[] territory_record_data;
        public Regex international_prefix;
        public string national_prefix;
        public Regex national_prefix_for_parsing;
        public string national_prefix_transform_rule;

        public Region(object data)
            : base(data)
        {
            country_code = field<string>(0);
            format_record_data = field<object[]>(1);
            territory_record_data = field<object[]>(2);
            international_prefix = field<string, Regex>(3, p => new Regex("^(?:" + p + ")"));
            national_prefix = field<string>(4);
            national_prefix_for_parsing = field<string, Regex>(5, p => new Regex("^(?:" + p + ")"));
            national_prefix_transform_rule = field<string>(6);
        }

        private IEnumerable<Territory> _territories;
        private IEnumerable<Format> _formats;

        public IEnumerable<Territory> territories()
        {
            return _territories ?? (_territories = territory_record_data.Map(data => new Territory(data, this)));
        }

        //          def formats
        public IEnumerable<Format> formats()
        {
            return _formats ?? (format_record_data.Map(data => new Format(data)));
        }


        //    end


        public Number parse_national_string(string @string)
        {
            @string = Number.normalize(@string);
            if (starts_with_country_code(@string))
            {
                @string = strip_country_code(@string);
                return find_first_parsed_national_string_from_territories(@string);
            }
            return null;
        }

        private Number find_first_parsed_national_string_from_territories(string s)
        {
            return Utils.map_detect(territories(), territory => territory.parse_national_string(s));
        }

        private string strip_country_code(string s)
        {
            return s.Substring(country_code.Length);
        }

        private bool starts_with_country_code(string s)
        {
            return s.StartsWith(country_code);
        }

        public Territory territory(string name)
        {
            name = name.ToUpper();
            return territories().detect(region => region.name == name);
        }

        public bool has_territory(string name)
        {
            return territory_names().Contains(name.ToUpper());
        }
        private List<string> territory_names()
        {
            return territory_record_data.Map(d => ((object[])d)[0].ToString().ToUpper()).ToList();
        }

    }
}