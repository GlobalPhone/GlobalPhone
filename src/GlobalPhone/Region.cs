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
            CountryCode = Field<string>(0);
            _formatRecordData = Field<object[]>(1);
            _territoryRecordData = Field<object[]>(2);
            InternationalPrefix = Field<string, Regex>(3, p => new Regex("^(?:" + p + ")"));
            NationalPrefix = Field<string>(4);
            NationalPrefixForParsing = Field<string, Regex>(5, p => new Regex("^(?:" + p + ")"));
            NationalPrefixTransformRule = Field<string>(6);
        }

        private IEnumerable<Territory> _territories;
        private IEnumerable<Format> _formats;

        public IEnumerable<Territory> Territories
        {
            get { return _territories ?? (_territories = _territoryRecordData.Map(data => new Territory(data, this))); }
        }

        public IEnumerable<Format> Formats
        {
            get { return _formats ?? (_formats=_formatRecordData.Map(data => new Format(data))); }
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
            return Territories.MapDetect(territory => territory.ParseNationalString(s));
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
            return Territories.Detect(region => region.Name == name);
        }

        public bool HasTerritory(string name)
        {
            return TerritoryNames().Contains(name.ToUpper());
        }
        private List<string> TerritoryNames()
        {
            return _territoryRecordData.Map(d => ((object[])d)[0].ToString().ToUpper()).ToList();
        }

    }
}