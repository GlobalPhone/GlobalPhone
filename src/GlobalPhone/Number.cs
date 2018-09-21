using System;
using System.Linq;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace GlobalPhone
{
    public class Number:IEquatable<Number>
    {
        private readonly PhoneNumber _num;
        private readonly PhoneNumberUtil _util;


        private static readonly Regex LeadingPlusChars = new Regex("^\\++", RegexOptions.Compiled);
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
                
                return areaCodeLength > 0
                    ? _util.GetNationalSignificantNumber(_num).Substring(0, areaCodeLength)
                    : "";
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
                int areaCodeLength = _util.GetLengthOfGeographicalAreaCode(_num);
                var nationalSignificantNumber = _util.GetNationalSignificantNumber(_num);
                return areaCodeLength > 0
                    ? nationalSignificantNumber.Substring(areaCodeLength)
                    : nationalSignificantNumber;
            }
        }

        /// <summary>
        /// For instance "771-79 33 36" of "+46 771 793 336"
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
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Number);
        }

        public bool Equals(Number obj)
        {
            if (ReferenceEquals(null, obj)){ return false; }
            return _num.Equals(obj._num);
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + _num.GetHashCode();
                return hash;
            }
        }
        
        public static bool operator ==(Number a, Number b)
        {
            if (ReferenceEquals(null, a)) return ReferenceEquals(null, b);
            return a.Equals(b);
        }

        public static bool operator !=(Number a, Number b)
        {
            return !(a == b);
        }
    }
}