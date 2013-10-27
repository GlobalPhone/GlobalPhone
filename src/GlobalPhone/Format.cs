using System;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Format : Record
    {
        private readonly Regex _pattern;
        private readonly string _nationalFormatRule;
        private readonly Regex _leadingDigits;
        public readonly string NationalPrefixFormattingRule;
        private readonly string _internationalFormatRule;

        public Format(object data)
            : base(data)
        {
            //name = field<string>(0);
            _pattern = Field<string, Regex>(0, p => new Regex("^" + p + "$"));
            _nationalFormatRule = Field<string>(1);
            _leadingDigits = Field<string, Regex>(2, p => new Regex("^" + p + ""));
            NationalPrefixFormattingRule = Field<string>(3);
            _internationalFormatRule = Field(4, fallback: _nationalFormatRule);
        }

        public bool Match(string nationalString, bool? matchLeadingDigits = null)
        {
            var m = matchLeadingDigits ?? true;
            if (m && _leadingDigits != null && !nationalString.Match(_leadingDigits).Success)
                return false;
            return nationalString.Match(_pattern).Success;
        }

        public string Apply(string nationalString, string type)
        {
            string replacement;
            if ((replacement = format_replacement_string(type)) != null)
            {
                return nationalString.Gsub(_pattern, replacement);
            }
            return null;
        }

        private string format_replacement_string(string type)
        {
            string formatRule;
            switch (type)
            {
                case "national":
                    formatRule = _nationalFormatRule;
                    break;
                case "international":
                    formatRule = _internationalFormatRule;
                    break;
                default:
                    throw new Exception(type);
            }
            if (formatRule == "NA")
                return null;
            return formatRule;
        }
    }
}