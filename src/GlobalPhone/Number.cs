using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Number
    {
        public Territory Territory { get; private set; }

        public string NationalString { get; private set; }
        public string NationalPrefix
        {
            get { return Territory.NationalPrefix; }
        }
        public string CountryCode
        {
            get { return Territory.CountryCode; }
        }
        public Region Region
        {
            get { return Territory.Region; }
        }
        public Regex NationalPattern
        {
            get { return Territory.NationalPattern; }
        }

        public string NationalFormat
        {
            get
            {
                return _nationalFormat ?? (_nationalFormat = Begin.Do(() =>
                    {
                        string result;
                        if (Format != null && (result = Format.Apply(NationalString, "national")) != null)
                        {
                            return ApplyNationalPrefixFormat(result);
                        }
                        return NationalString;
                    }));
            }
        }
        private string NationalPrefixFormattingRule
        {
            get { return Format.NationalPrefixFormattingRule ?? Territory.NationalPrefixFormattingRule; }
        }
        private string ApplyNationalPrefixFormat(string result)
        {
            var prefix = NationalPrefixFormattingRule;
            Match match;
            if (!string.IsNullOrEmpty(prefix) && (match = SplitFirstGroup.Match(result ?? String.Empty)).Success)
            {
                prefix = prefix.Replace("$NP", NationalPrefix);
                prefix = prefix.Replace("$FG", match.Groups[1].Value);
                result = prefix + " " + match.Groups[2].Value;
                return result;
            }
            return result;
        }

        public bool IsValid
        {
            get { return Format != null && NationalPattern.Match(NationalString ?? String.Empty).Success; }
        }

        internal Format Format
        {
            get { return _format ?? (_format = FindFormatFor(NationalString)); }
        }

        public string InternationalFormat
        {
            get
            {
                return _internationalFormat ?? (_internationalFormat =
                    Begin.Do(() =>
                                {
                                    string formattedNumber;
                                    if (Format != null &&
                                        (formattedNumber =
                                        Format.Apply(NationalString, "international")) != null)
                                    {
                                        return "+" + CountryCode + " " + formattedNumber;
                                    }
                                    return "+" + CountryCode + " " + NationalString;
                                }));
            }
        }

        private string _internationalString;

        public string InternationalString
        {
            get { return _internationalString ?? (_internationalString = NonDialableChars.Replace(InternationalFormat, "")); }
        }

        private Format FindFormatFor(string str)
        {
            return Region.Formats.FirstOrDefault(f => f.Match(str))
            ?? Region.Formats.FirstOrDefault(f => f.Match(str, false));
        }

        private static readonly Regex LeadingPlusChars = new Regex("^\\++", RegexOptions.Compiled);
        private static readonly Regex NonDialableChars = new Regex("[^,#+\\*\\d]", RegexOptions.Compiled);
        private static readonly Regex SplitFirstGroup = new Regex("^(\\d+)\\W*(.*)$", RegexOptions.Compiled);
        private Format _format;
        private string _internationalFormat;
        private string _nationalFormat;

        public Number(Territory territory, string nationalString)
        {
            Territory = territory;
            NationalString = nationalString;
        }

        protected internal static string Normalize(string str)
        {
            return (str ?? String.Empty)
                    .Yield(s=>LeadingPlusChars.Replace(s, "+"))
                    .Yield(s=>NonDialableChars.Replace(s, ""));
        }
        public override string ToString()
        {
            return InternationalString;
        }
        private static readonly Regex notSlashD = new Regex(@"[^\d]"); 
        public string AreaCode
        {
            get
            {

                if (NationalPrefixFormattingRule != null)
                {
                    var areaCodeSuffix = SplitFirstGroup.Match(FormattedNationalString).Groups[1].Value;
                    var formattedNationalPrefix = NationalPrefixFormattingRule.Replace("$NP", NationalPrefix).Replace("$FG", areaCodeSuffix);
                    return notSlashD.Replace(formattedNationalPrefix, "");
                }
                return null;
            }
        }

        private string FormattedNationalString
        {
            get
            {
                return Format.Apply(NationalString, "national");
            }
        }

        public string LocalNumber
        {
            get
            {
                return AreaCode != null ? SplitFirstGroup.Match(FormattedNationalString).Groups[2].Value : NationalFormat;
            }
        }
    }
}