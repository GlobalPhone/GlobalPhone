using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Number
    {
        //            attr_reader :territory, :national_string
        public Territory territory { get; set; }
        /*    attr_reader :territory, :national_string

    def_delegator :territory, :region
    def_delegator :territory, :country_code
    def_delegator :territory, :national_prefix
    def_delegator :territory, :national_pattern
*/

        public string national_string { get; set; }
        public string national_prefix
        {
            get { return territory.national_prefix; }
        }
        public string country_code
        {
            get { return territory.country_code; }
        }
        public Region region
        {
            get { return territory.region; }
        }
        public Regex national_pattern
        {
            get { return territory.national_pattern; }
        }

        public string national_format
        {
            get
            {
                return _national_format ?? (_national_format = Begin.Do(() =>
                    {
                        string result;
                        if (format!=null && (result=format.apply(national_string,"national"))!=null)
                        {
                            return apply_national_prefix_format(result);
                        }else
                        {
                            return national_string;
                        }
                    }));
            }
        }
        private string national_prefix_formatting_rule
        {
            get { return format.national_prefix_formatting_rule ?? territory.national_prefix_formatting_rule; }
        }
        private string apply_national_prefix_format(string result)
        {
            var prefix = national_prefix_formatting_rule;
            Match match;
            if (!string.IsNullOrEmpty(prefix) && (match = result.match(SPLIT_FIRST_GROUP)).Success)
            {
                prefix = prefix.Replace("$NP", national_prefix);
                prefix = prefix.Replace("$FG", match.Groups[1].Value);
                result = prefix + " " + match.Groups[2].Value;
                return result;
            }else
            {
                return result;
            }
        }

        public bool valid()
        {
            return format != null && national_string.match(national_pattern).Success;
        }

        private Format format
        {
            get { return _format ?? (_format = find_format_for(national_string)); }
        }

        public string international_format
        {
            get
            {
                return _international_format ?? (_international_format =
                    Begin.Do(() =>
                                {
                                    string formatted_number;
                                    if (format != null &&
                                        (formatted_number =
                                        format.apply(national_string, "international")) != null)
                                    {
                                        return "+" + country_code + " " + formatted_number;
                                    }
                                    else
                                    {
                                        return "+" + country_code + " " + national_string;
                                    }
                                }));
            }
        }

        private string _international_string;

        public string international_string
        {
            get { return _international_string ?? (_international_string = international_format.gsub(NON_DIALABLE_CHARS, "")); }
        }

        private Format find_format_for(string str)
        {
            return region.formats().detect(f => f.match(str))
            ?? region.formats().detect(f => f.match(str, false));
        }

        private static Dictionary<string, string> E161_MAPPING = "a2b2c2d3e3f3g4h4i4j5k5l5m6n6o6p7q7r7s7t8u8v8w9x9y9z9".SplitN(2).ToDictionary(kv => kv[0].ToString(), kv => kv[1].ToString());
        private static Regex VALID_ALPHA_CHARS = new Regex("[a-zA-Z]");
        private static Regex LEADING_PLUS_CHARS = new Regex("^\\++");
        private static Regex NON_DIALABLE_CHARS = new Regex("[^,#+\\*\\d]");
        private static Regex SPLIT_FIRST_GROUP = new Regex("^(\\d+)(.*)$");
        private Format _format;
        private string _international_format;
        private string _national_format;

        public Number(Territory territory, string national_string)
        {
            this.territory = territory;
            this.national_string = national_string;
        }

        public static string normalize(string str)
        {
            return
                str.gsub(VALID_ALPHA_CHARS, match => 
                    E161_MAPPING[match.Value.ToLower()])
                    .gsub(LEADING_PLUS_CHARS, "+")
                    .gsub(NON_DIALABLE_CHARS, "");
        }
        public override string ToString()
        {
            return international_string;
        }
    }
}