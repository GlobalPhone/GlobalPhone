using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Territory : Record
    {
        private readonly Region _region;
        public readonly string Name;
        private readonly Regex _possiblePattern;
        public readonly Regex NationalPattern;
        public readonly string NationalPrefixFormattingRule;

        public Territory(object data, Region region)
            : base(data)
        {
            _region = region;
            Name = Field<string>(0);
            _possiblePattern = Field<string, Regex>(1, p => new Regex("^" + p + "$"));
            NationalPattern = Field<string, Regex>(2, p => new Regex("^" + p + "$"));
            NationalPrefixFormattingRule = Field<string>(3);
        }

        public string CountryCode
        {
            get { return _region.CountryCode; }
        }
        public Regex InternationalPrefix
        {
            get { return _region.InternationalPrefix; }
        }
        public string NationalPrefix
        {
            get { return _region.NationalPrefix; }
        }
        public Regex NationalPrefixForParsing
        {
            get { return _region.NationalPrefixForParsing; }
        }

        public string NationalPrefixTransformRule
        {
            get { return _region.NationalPrefixTransformRule; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public Number parse_national_string(string str)
        {
            str = Normalize(str);
            if (Possible(str))
                return new Number(this, str);
            return null;
        }

        private bool Possible(string str)
        {
            return str.Match(_possiblePattern).Success;
        }

        protected string Normalize(string str)
        {
            return StripNationalPrefix(Number.Normalize(str));
        }
        protected string StripNationalPrefix(string str)
        {
            string stringWithoutPrefix = null;
            if (NationalPrefixForParsing != null)
            {
                var transformRule = NationalPrefixTransformRule ?? "";
                stringWithoutPrefix = str.Sub(NationalPrefixForParsing, transformRule);
            }
            else if (starts_with_national_prefix(str))
            {
                stringWithoutPrefix = str.Substring(NationalPrefix.Length);
            }
            return Possible(stringWithoutPrefix) ? stringWithoutPrefix : str;
        }
        protected bool starts_with_national_prefix(string str)
        {
            return NationalPrefix != null && str.StartsWith(NationalPrefix);
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
            return Equals(other.Name, Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}