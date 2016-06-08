using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Number
    {
        /// <summary>
        /// Get the territory for this instance.
        /// </summary>
        public Territory Territory { get; private set; }
        /// <summary>
        /// For instance 3125551212 of "(312) 555-1212" or 771793336 of "+46 771 793 336"
        /// </summary>
        public string NationalString { get; private set; }
        /// <summary>
        /// Gets the country for the code.
        /// For instance for a swedish number you will get 46, for a US number you will get 1.
        /// </summary>
        /// <value>The country code.</value>
        public string CountryCode
        {
            get { return Territory.Region.CountryCode; }
        }
        /// <summary>
        /// Get the region for the this instances territory.
        /// </summary>
        public Region Region
        {
            get { return Territory.Region; }
        }
        /// <summary>
        /// For instance (312) 555-1212 for the number 312-555-1212 (us)
        /// or "0771 79 33 36" for the number +46771793336
        /// </summary>
        /// <value>The national format.</value>
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
                result = string.Concat( prefix
                    .Replace("$NP", Territory.Region.NationalPrefix)
                    .Replace("$FG", match.Groups[1].Value),
                    " ", match.Groups[2].Value);
                return result;
            }
            return result;
        }

        /// <summary>
        /// If this instance is a valid number for it's territory.
        /// </summary>
        public bool IsValid
        {
            get { return Format != null && Territory.NationalPatternMatch(NationalString); }
        }

        private Format Format
        {
            get { return _format ?? (_format = FindFormatFor(NationalString)); }
        }
        /// <summary>
        /// Gets the international format. For instance "+1 312-555-1212" or "+46 771 79 33 36".
        /// </summary>
        /// <value>The international format.</value>
        public string InternationalFormat
        {
            get
            {
                return _internationalFormat ?? (_internationalFormat =
                    Begin.Do(() =>
                                {
                                    string formattedNumber;
                                    if (Format != null &&
                                        (formattedNumber = Format.Apply(NationalString, "international")) != null)
                                    {
                                        return string.Concat("+", CountryCode, " ", formattedNumber);
                                    }
                                    return string.Concat("+", CountryCode, " ", NationalString);
                                }));
            }
        }

        private string _internationalString;

        /// <summary>
        /// Gets the international string. For instance "+13125551212" or "+46771793336".
        /// </summary>
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

        internal Number(Territory territory, string nationalString)
        {
            Territory = territory;
            NationalString = nationalString;
        }
        /// <summary>
        /// Normalize the specified str based on territory.
        /// if null is sent in as territory, assumes that E161 is not used.
        /// </summary>
        /// <param name="str">a number</param>
        /// <param name="territory">Territory to identify if you should use E161.</param>
        protected internal static string Normalize(string str, Territory territory)
        {
            str = str ?? String.Empty;
            return (territory != null && E161.UsedBy(territory)
                    ? E161.Normalize(str)
                    : str)
                    .Yield(s => LeadingPlusChars.Replace(s, "+"))
                    .Yield(s => NonDialableChars.Replace(s, ""));
        }
        /// <summary>
        /// Returns the InternationalString for this instance.
        /// </summary>
        public override string ToString()
        {
            return InternationalString;
        }
        private static readonly Regex notSlashD = new Regex(@"[^\d]");
        /// <summary>
        /// Gets the area code. For instance the "702" part of "+1 702-389-1234".
        /// </summary>
        /// <value>The area code.</value>
        public string AreaCode
        {
            get
            {
                if (NationalPrefixFormattingRule != null)
                {
                    var areaCodeSuffix = SplitFirstGroup.Match(FormattedNationalString).Groups[1].Value;
                    var formattedNationalPrefix = NationalPrefixFormattingRule
                        .Replace("$NP", Territory.Region.NationalPrefix)
                        .Replace("$FG", areaCodeSuffix);
                    return notSlashD.Replace(formattedNationalPrefix, "");
                }
                return Format.FirstInPattern(NationalString);
            }
        }
        /// <summary>
        /// For instance "771-79 33 36" of "+46 771 793 336"
        /// or "(650) 253-0000" of "+1 650-253-0000"
        /// </summary>
        public string FormattedNationalString
        {
            get
            {
                return Format.Apply(NationalString, "national");
            }
        }
        /// <summary>
        /// Should return 0771793336 of +46 771 793 336 and 3125551212 of the US number (312) 555-1212
        /// </summary>
        public string NationalNumber
        {
            get
            {
                // I don't know if this is correct
                if (Territory.NationalPrefixFormattingRule != null)
                {
                    return Territory.NationalPrefix + NationalString;
                }
                return NationalString;
            }
        }

        /// <summary>
        /// Gets the local number.
        /// For instance the "9876 0010" part of "+61 3 9876 0010"
        /// or the "79 33 36" part of "+46 771 793 336"
        /// </summary>
        /// <value>The local number.</value>
        public string LocalNumber
        {
            get
            {
                return AreaCode != null ? SplitFirstGroup.Match(FormattedNationalString).Groups[2].Value : NationalFormat;
            }
        }
    }
}