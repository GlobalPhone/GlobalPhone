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
            _pattern = Field<string, Regex>(0, block: p => new Regex("^" + p + "$"), column: "pattern");
            _nationalFormatRule = Field<string>(1, column: "format");
            _leadingDigits = Field<string, Regex>(2, block: p => new Regex("^" + p + ""), column: "leadingDigits");
            NationalPrefixFormattingRule = Field<string>(3, column: "formatRule");
            _internationalFormatRule = Field(4, fallback: _nationalFormatRule, column: "intlFormat");
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
            if ((replacement = FormatReplacementString(type)) != null)
            {
                return nationalString.Gsub(_pattern, replacement);
            }
            return null;
        }

        private string FormatReplacementString(string type)
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