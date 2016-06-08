using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Region : Record
    {
        /// <summary>
        /// Gets the country code.
        /// For instance for a swedish number you will get 46, for a US number you will get 1.
        /// </summary>
        public string CountryCode;
        private readonly object[] _formatRecordData;
        private readonly object[] _territoryRecordData;
        private Regex _internationalPrefix;
        public string NationalPrefix;
        private Regex _nationalPrefixForParsing;
        public string NationalPrefixTransformRule;

        public Region(object data)
            : base(data)
        {
            CountryCode = Field<string>(column: "countryCode");
            _formatRecordData = FieldAsArray(column: "formats");
            _territoryRecordData = FieldAsArray(column: "territories");
            _internationalPrefix = Field<string, Regex>(column: "interPrefix", block: p => new Regex("^(?:" + p + ")"));
            NationalPrefix = Field<string>(column: "prefix");
            _nationalPrefixForParsing = Field<string, Regex>(column: "prefixParse", block: p => new Regex("^(?:" + p + ")$"));
            NationalPrefixTransformRule = Field<string>(column: "prefixTRule");
        }

        private IEnumerable<Territory> _territories;
        private IEnumerable<Format> _formats;

        public IEnumerable<Territory> Territories
        {
            get { return _territories ?? (_territories = _territoryRecordData.Select(data => new Territory(data, this))); }
        }

        public IEnumerable<Format> Formats
        {
            get { return _formats ?? (_formats = _formatRecordData.Select(data => new Format(data))); }
        }

        public Number ParseNationalString(string @string)
        {
            @string = Number.Normalize(@string, null);
            if (StartsWithCountryCode(@string))
            {
                @string = StripCountryCode(@string);
                return FindFirstParsedNationalStringFromTerritories(@string);
            }
            throw new FailedToParseNumberException();
        }

        private Number FindFirstParsedNationalStringFromTerritories(string s)
        {
            return Territories.SelectWhereNotNull(territory => territory.ParseNationalString(s));
        }

        private string StripCountryCode(string s)
        {
            return s.Substring(CountryCode.Length);
        }

        public bool TryStripNationalPrefix(string str, out string stringWithoutPrefix)
        {
            if (_nationalPrefixForParsing != null)
            {
                var transformRule = NationalPrefixTransformRule ?? "";
                stringWithoutPrefix = _nationalPrefixForParsing.Replace(str, transformRule, 1);
                return true;
            }
            else if (StartsWithNationalPrefix(str))
            {
                stringWithoutPrefix = str.Substring(NationalPrefix.Length);
                return true;
            }
            stringWithoutPrefix = null;
            return false;
        }

        private bool StartsWithNationalPrefix(string str)
        {
            return NationalPrefix != null && str.StartsWith(NationalPrefix);
        }


        private bool StartsWithCountryCode(string s)
        {
            return s.StartsWith(CountryCode);
        }

        public Territory Territory(string name)
        {
            name = name.ToUpper();
            return Territories.FirstOrDefault(region => region.Name == name);
        }

        public bool HasTerritory(string name)
        {
            return TerritoryNames().Contains(name.ToUpper());
        }
        private List<string> TerritoryNames()
        {
            return _territoryRecordData.Select(d => IsArray(d) ? (AsArray(d))[0].ToString().ToUpper() : (AsHash(d))["name"].ToString().ToUpper()).ToList();
        }

        public bool TryStripInternationalPrefix(string str, out string stripped)
        {
            if (_internationalPrefix.Match(str ?? string.Empty).Success)
            {
                stripped = StripInternationalPrefix(str);
                return true;
            }
            stripped = null;
            return false;
        }

        private string StripInternationalPrefix(string @string)
        {
            return _internationalPrefix.Replace(@string, "");
        }
    }
}