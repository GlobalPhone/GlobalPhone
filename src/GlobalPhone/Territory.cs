using System;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Territory : Record
    {
        private readonly Region _region;
        public string name;
        private Regex possible_pattern;
        public Regex national_pattern;
        public string national_prefix_formatting_rule;

        public Territory(object data, Region region)
            : base(data)
        {
            _region = region;
            name = field<string>(0);
            possible_pattern = field<string, Regex>(1, p => new Regex("^" + p + "$"));
            national_pattern = field<string, Regex>(2, p => new Regex("^" + p + "$"));
            national_prefix_formatting_rule = field<string>(3);
        }

        public string country_code
        {
            get { return _region.country_code; }
        }
        public Regex international_prefix
        {
            get { return _region.international_prefix; }
        }
        public string national_prefix
        {
            get { return _region.national_prefix; }
        }
        public Regex national_prefix_for_parsing
        {
            get { return _region.national_prefix_for_parsing; }
        }

        public string national_prefix_transform_rule
        {
            get { return _region.national_prefix_transform_rule; }
        }

        public Region region
        {
            get { return _region; }
        }

        public Number parse_national_string(string str)
        {
            str = normalize(str);
            if (possible(str))
                return new Number(this, str);
            return null;
        }

        private bool possible(string str)
        {
            return str.match(possible_pattern).Success;
        }

        protected string normalize(string str)
        {
            return strip_national_prefix(Number.normalize(str));
        }
        protected string strip_national_prefix(string str)
        {
            string string_without_prefix = null;
            if (national_prefix_for_parsing != null)
            {
                var transform_rule = national_prefix_transform_rule ?? "";
                string_without_prefix = str.sub(national_prefix_for_parsing, transform_rule);
            }
            else if (starts_with_national_prefix(str))
            {
                string_without_prefix = str.Substring(national_prefix.Length);
            }
            return possible(string_without_prefix) ? string_without_prefix : str;
        }
        protected bool starts_with_national_prefix(string str)
        {
            return national_prefix != null && str.StartsWith(national_prefix);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Territory)) return false;
            return Equals((Territory) obj);
        }

        public bool Equals(Territory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.name, name);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }
        public override string ToString()
        {
            return name;
        }
    }
}