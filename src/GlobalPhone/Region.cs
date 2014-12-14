using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    public class Region : Record
    {
        public string CountryCode;
        private readonly object[] _formatRecordData;
        private readonly object[] _territoryRecordData;
        public Regex InternationalPrefix;
        public string NationalPrefix;
        public Regex NationalPrefixForParsing;
        public string NationalPrefixTransformRule;

        public Region(object data)
            : base(data)
        {
            CountryCode = Field<string>(0, column: "countryCode");
            _formatRecordData = FieldAsArray(1, column: "formats");
            _territoryRecordData = FieldAsArray(2, column: "territories");
            InternationalPrefix = Field<string, Regex>(3, column: "interPrefix", block: p => new Regex("^(?:" + p + ")"));
            NationalPrefix = Field<string>(4, column: "prefix");
            NationalPrefixForParsing = Field<string, Regex>(5, column: "prefixParse", block: p => new Regex("^(?:" + p + ")$"));
            NationalPrefixTransformRule = Field<string>(6, column: "prefixTRule");
        }

        private IEnumerable<Territory> _territories;
        private IEnumerable<Format> _formats;

        public IEnumerable<Territory> Territories
        {
            get { return _territories ?? (_territories = _territoryRecordData.Select(data => new Territory(data, this))); }
        }

        public IEnumerable<Format> Formats
        {
            get { return _formats ?? (_formats=_formatRecordData.Select(data => new Format(data))); }
        }

        public Number ParseNationalString(string @string)
        {
            @string = Number.Normalize(@string);
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

    }
}