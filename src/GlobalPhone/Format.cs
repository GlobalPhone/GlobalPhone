using System;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Format : Record
    {
        private Regex pattern;
        private string national_format_rule;
        private Regex leading_digits;
        public string national_prefix_formatting_rule;
        private string international_format_rule;

        public Format(object data)
            : base(data)
        {
            //name = field<string>(0);
            pattern = field<string, Regex>(0, p => new Regex("^" + p + "$"));
            national_format_rule = field<string>(1);
            leading_digits = field<string, Regex>(2, p => new Regex("^" + p + ""));
            national_prefix_formatting_rule = field<string>(3);
            international_format_rule = field<string>(4, fallback: national_format_rule);
        }

        public bool match(string national_string, bool? match_leading_digits = null)
        {
            var m = match_leading_digits ?? true;
            if (m && leading_digits != null && !national_string.match(leading_digits).Success)
                return false;
            return national_string.match(pattern).Success;

            /*    def match(national_string, match_leading_digits = true)
      return false if match_leading_digits && leading_digits && national_string !~ leading_digits
      national_string =~ pattern
    end
*/
        }

        public string apply(string national_string, string type)
        {
            string replacement;
            if ((replacement = format_replacement_string(type)) != null)
            {
                return national_string.gsub(pattern, replacement);
            }
            return null;
        }

        private string format_replacement_string(string type)
        {
            var format_rule = "";
            switch (type)
            {
                case "national":
                    format_rule = national_format_rule;
                    break;
                case "international":
                    format_rule = international_format_rule;
                    break;
                default:
                    throw new Exception(type);
            }
            if (format_rule == "NA")
                return null;
            return format_rule;
        }
    }
}