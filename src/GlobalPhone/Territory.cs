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
        public readonly bool NationalPrefixOptionalWhenFormatting;
        public readonly string NationalPrefix;

        public Territory(object data, Region region)
            : base(data)
        {
            _region = region;
            Name = Field<string>(column: "name");
            _possiblePattern = Field<string, Regex>(column: "possibleNumber", block: p => new Regex("^" + p + "$"));
            _nationalPattern = Field<string, Regex>(column: "nationalNumber", block: p => new Regex("^" + p + "$"));
            NationalPrefixFormattingRule = Field<string>(column: "formattingRule");
            NationalPrefix = Field<string>(column: "nationalPrefix");
            NationalPrefixOptionalWhenFormatting = Field<bool>(column: "nationalPrefixOptionalWhenFormatting");
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
            return Number.Normalize(str, this);
        }

        private string ToNationalNumber(string str)
        {
            return StripNationalPrefix(Number.Normalize(str, this));
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
                return Possible(stringWithoutPrefix) ? stringWithoutPrefix : str;
            }
            return str;
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