using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace GlobalPhone
{
    /// <summary>
    /// A wrapper class around <see cref="PhoneNumbers.PhoneNumber"/> in order to provide additional functionality. It has a
    /// reference to <see cref="PhoneNumberUtil"/> in order to be able to give a nicer API.
    /// The downside is that this implies that there are more references per number. This might not matter
    /// in many scenarios.
    /// </summary>
    public class Number:IEquatable<Number>
    {
        /// <summary>
        /// Get the wrapped phone number
        /// </summary>
        public readonly PhoneNumber PhoneNumber;
        private readonly PhoneNumberUtil _util;

        private static readonly Regex NonDialableChars = new Regex("[^,#+\\*\\d]", RegexOptions.Compiled);
        private string _nationalFormat;
        private string _internationalFormat;
        private string _internationalString;

        internal Number(PhoneNumber phoneNumber, PhoneNumberUtil util)
        {
            PhoneNumber = phoneNumber;
            _util = util;
        }

        public ulong NationalNumber => PhoneNumber.NationalNumber;
        /// <summary>
        /// Gets the area code. For instance the "702" part of "+1 702-389-1234".
        /// </summary>
        /// <value>The area code.</value>
        public string AreaCode
        {
            get
            {
                var areaCodeLength = _util.GetLengthOfGeographicalAreaCode(PhoneNumber);
                var region = _util.GetRegionCodeForNumber(PhoneNumber);
                var metadata = _util.GetMetadataForRegion(region);
                
                if (areaCodeLength > 0)
                {
                    var values = new List<string>();
                    if (metadata.HasNationalPrefix && !_util.IsNANPACountry(region))
                    {
                        values.Add(metadata.NationalPrefix);
                    }
                    
                    values.Add(_util.GetNationalSignificantNumber(PhoneNumber).Substring(0, areaCodeLength));
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
                var areaCodeLength = _util.GetLengthOfGeographicalAreaCode(PhoneNumber);
                var nationalSignificantNumber = _util.GetNationalSignificantNumber(PhoneNumber);
                
                return areaCodeLength > 0
                    ? nationalSignificantNumber.Substring(areaCodeLength)
                    : nationalSignificantNumber;
            }
        }

        /// <summary>
        /// Metadata for number
        /// </summary>
        public PhoneMetadata PhoneMetadata =>
            _util.GetMetadataForRegion(_util.GetRegionCodeForNumber(PhoneNumber));

        /// <summary>
        /// Region code for number
        /// </summary>
        public string RegionCode =>
            _util.GetRegionCodeForNumber(PhoneNumber);


        /// <summary>
        /// For instance "0771-79 33 36" of "+46 771 793 336"
        /// or "(650) 253-0000" of "+1 650-253-0000"
        /// </summary>
        public string FormattedNationalString => _util.Format(PhoneNumber, PhoneNumberFormat.NATIONAL);

        /// <summary>
        /// For instance (312) 555-1212 for the number 312-555-1212 (us)
        /// or "0771 79 33 36" for the number +46771793336
        /// </summary>
        /// <value>The national format.</value>
        public string NationalFormat =>
            _nationalFormat ?? (_nationalFormat = _util.Format(PhoneNumber, PhoneNumberFormat.NATIONAL));
        /// <summary>
        /// Gets the international format. For instance "+1 312-555-1212" or "+46 771 79 33 36".
        /// </summary>
        /// <value>The international format.</value>
        public string InternationalFormat => _internationalFormat ??
                                             (_internationalFormat =
                                                 _util.Format(PhoneNumber, PhoneNumberFormat.INTERNATIONAL));

        public int CountryCode => PhoneNumber.CountryCode;

        public bool IsValid => _util.IsValidNumber(PhoneNumber);
        /// <summary>
        /// For instance 3125551212 of "(312) 555-1212" or 0771793336 of "+46 771 793 336"
        /// </summary>
        public string NationalString =>
            _util.Format(PhoneNumber, PhoneNumberFormat.NATIONAL)
                .Yield(s => NonDialableChars.Replace(s, ""));

        /// <summary>
        /// Gets the international string. For instance "+13125551212" or "+46771793336".
        /// </summary>
        public string InternationalString => _internationalString ??
                                             (_internationalString = NonDialableChars.Replace(InternationalFormat, ""));
        
        public override bool Equals(object obj) => Equals(obj as Number);

        public bool Equals(Number obj) => !ReferenceEquals(null, obj) && PhoneNumber.Equals(obj.PhoneNumber);

        public override int GetHashCode() => PhoneNumber.GetHashCode();

        public override string ToString() => InternationalString;

        public static bool operator ==(Number a, Number b) => a?.Equals(b) ?? ReferenceEquals(null, b);

        public static bool operator !=(Number a, Number b) => !(a == b);
    }
}