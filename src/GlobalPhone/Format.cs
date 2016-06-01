using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Format : Record
    {
        private readonly Regex _pattern;
        private readonly string _nationalFormatRule;
        private readonly Regex[] _leadingDigits;
        public readonly string NationalPrefixFormattingRule;
        private readonly string _internationalFormatRule;
        internal string Pattern { get { return _pattern.ToString(); } }

        public Format(object data)
            : base(data)
        {
            //name = field<string>(0);
            _pattern = Field<string, Regex>(0, block: p => new Regex("^" + p + "$"), column: "pattern");
            _nationalFormatRule = Field<string>(1, column: "format");
            _leadingDigits = FieldMaybeAsArray<string, Regex>(2, block: p => new Regex("^" + p + ""), column: "leadingDigits");
            NationalPrefixFormattingRule = Field<string>(3, column: "formatRule");
            _internationalFormatRule = Field(4, fallback: _nationalFormatRule, column: "intlFormat");
        }

        public bool Match(string nationalString, bool? matchLeadingDigits = null)
        {
            var matchesNational = _pattern.Match(nationalString ?? String.Empty).Success;
            if (matchLeadingDigits ?? true)
                return matchesNational && MatchLeadingDigits(nationalString);
            return matchesNational;
        }

        private bool MatchLeadingDigits(string nationalString)
        {
            return _leadingDigits != null && _leadingDigits.Any()
                && _leadingDigits.Any(MatchSuccess(nationalString));
        }

        private static Func<Regex, bool> MatchSuccess(string nationalString)
        {
            return leadingDigits => leadingDigits.Match(nationalString ?? String.Empty).Success;
        }

        public string Apply(string nationalString, string type)
        {
            string replacement;
            if ((replacement = FormatReplacementString(type)) != null)
            {
                return Replace(_pattern, nationalString, replacement);
            }
            return null;
        }

        internal static string Replace(Regex regex, string self, string evaluator)
        {
            if (evaluator.Contains("$"))
            {
                return regex.Replace(self, match =>
                    {
                        var eval = evaluator;
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            var g = match.Groups[i];
                            if (g.Success)
                            {
                                eval = eval.Replace("$" + (i), g.Value);
                            }
                        }
                        return eval;
                    });
            }
            return regex.Replace(self, evaluator);
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