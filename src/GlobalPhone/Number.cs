using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace GlobalPhone
{
    public class Number:IEquatable<Number>
    {
        private readonly PhoneNumber _num;
        private readonly PhoneNumberUtil _util;

        private static readonly Regex NonDialableChars = new Regex("[^,#+\\*\\d]", RegexOptions.Compiled);
        private string _nationalFormat;
        private string _internationalFormat;
        private string _internationalString;

        internal Number(PhoneNumber num, PhoneNumberUtil util)
        {
            _num = num;
            _util = util;
        }

        public ulong NationalNumber => _num.NationalNumber;
        /// <summary>
        /// Gets the area code. For instance the "702" part of "+1 702-389-1234".
        /// </summary>
        /// <value>The area code.</value>
        public string AreaCode
        {
            get
            {
                var areaCodeLength = _util.GetLengthOfGeographicalAreaCode(_num);
                var region = _util.GetRegionCodeForNumber(_num);
                var metadata = _util.GetMetadataForRegion(region);
                
                if (areaCodeLength > 0)
                {
                    var values = new List<string>();
                    if (metadata.HasNationalPrefix && !_util.IsNANPACountry(region))
                    {
                        values.Add(metadata.NationalPrefix);
                    }
                    
                    values.Add(_util.GetNationalSignificantNumber(_num).Substring(0, areaCodeLength));
                    return string.Concat(values);
                }
                return "";
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
                var areaCodeLength = _util.GetLengthOfGeographicalAreaCode(_num);
                var nationalSignificantNumber = _util.GetNationalSignificantNumber(_num);
                
                return areaCodeLength > 0
                    ? nationalSignificantNumber.Substring(areaCodeLength)
                    : nationalSignificantNumber;
            }
        }

        /// <summary>
        /// For instance "0771-79 33 36" of "+46 771 793 336"
        /// or "(650) 253-0000" of "+1 650-253-0000"
        /// </summary>
        public string FormattedNationalString => _util.Format(_num, PhoneNumberFormat.NATIONAL);

        /// <summary>
        /// For instance (312) 555-1212 for the number 312-555-1212 (us)
        /// or "0771 79 33 36" for the number +46771793336
        /// </summary>
        /// <value>The national format.</value>
        public string NationalFormat =>
            _nationalFormat ?? (_nationalFormat = _util.Format(_num, PhoneNumberFormat.NATIONAL));
        /// <summary>
        /// Gets the international format. For instance "+1 312-555-1212" or "+46 771 79 33 36".
        /// </summary>
        /// <value>The international format.</value>
        public string InternationalFormat => _internationalFormat ??
                                             (_internationalFormat =
                                                 _util.Format(_num, PhoneNumberFormat.INTERNATIONAL));

        public int CountryCode => _num.CountryCode;

        public bool IsValid => _util.IsValidNumber(_num);
        /// <summary>
        /// For instance 3125551212 of "(312) 555-1212" or 0771793336 of "+46 771 793 336"
        /// </summary>
        public string NationalString =>
            _util.Format(_num, PhoneNumberFormat.NATIONAL)
                .Yield(s => NonDialableChars.Replace(s, ""));

        /// <summary>
        /// Gets the international string. For instance "+13125551212" or "+46771793336".
        /// </summary>
        public string InternationalString => _internationalString ??
                                             (_internationalString = NonDialableChars.Replace(InternationalFormat, ""));
        
        public override bool Equals(object obj) => Equals(obj as Number);

        public bool Equals(Number obj) => !ReferenceEquals(null, obj) && _num.Equals(obj._num);

        public override int GetHashCode() => _num.GetHashCode();

        public override string ToString() => InternationalString;

        public static bool operator ==(Number a, Number b) => a?.Equals(b) ?? ReferenceEquals(null, b);

        public static bool operator !=(Number a, Number b) => !(a == b);
    }
}