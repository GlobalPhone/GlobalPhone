using System;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Territory : Record
    {
        private readonly Region _region;
        public readonly string Name;
        private readonly Regex _possiblePattern;
        private readonly Regex _nationalPattern;
        public readonly string NationalPrefixFormattingRule;

        public Territory(object data, Region region)
            : base(data)
        {
            _region = region;
            Name = Field<string>(0, column: "name");
            _possiblePattern = Field<string, Regex>(1, column: "possibleNumber", block: p => new Regex("^" + p + "$"));
            _nationalPattern = Field<string, Regex>(2, column: "nationalNumber", block: p => new Regex("^" + p + "$"));
            NationalPrefixFormattingRule = Field<string>(3, column: "formattingRule");
        }

        public string CountryCode
        {
            get { return _region.CountryCode; }
        }
        public string NationalPrefix
        {
            get { return _region.NationalPrefix; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public Number ParseNationalString(string str)
        {
            str = ToNationalNumber(str);
            if (Possible(str))
                return new Number(this, str);
            throw new FailedToParseNumberException("not possible for " + Name);
        }

        private bool Possible(string str)
        {
            return _possiblePattern.Match(str ?? string.Empty).Success;
        }

        public string Normalize(string str)
        {
            return Number.Normalize( (E161.UsedBy(this)) ? E161.Normalize(str) : str );
        }

        private string ToNationalNumber(string str)
        {
            return StripNationalPrefix(Number.Normalize(str));
        }

        internal bool NationalPatternMatch(string nationalString)
        {
            return _nationalPattern.Match(nationalString ?? string.Empty).Success;
        }

        private string StripNationalPrefix(string str)
        {
            string stringWithoutPrefix = null;
            if (_region.TryStripNationalPrefix(str, out stringWithoutPrefix))
            {

            }
            else if (StartsWithNationalPrefix(str))
            {
                stringWithoutPrefix = str.Substring(NationalPrefix.Length);
            }
            return Possible(stringWithoutPrefix) ? stringWithoutPrefix : str;
        }

        protected bool StartsWithNationalPrefix(string str)
        {
            return NationalPrefix != null && str.StartsWith(NationalPrefix);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Territory)) return false;
            return Equals((Territory)obj);
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