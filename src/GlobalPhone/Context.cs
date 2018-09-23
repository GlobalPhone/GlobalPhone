using PhoneNumbers;

namespace GlobalPhone
{
    public class Context
    {
        private readonly PhoneNumberUtil _util;

        public Context()
            : this(PhoneNumberUtil.GetInstance())
        {
        }

        public Context(PhoneNumberUtil util)
        {
            _util = util;
        }

        public string DefaultTerritoryName { get; set; }

        public Number Parse(string str, string territoryName = null)
        {
            var num = _util.Parse(str, (territoryName ?? DefaultTerritoryName)?.ToUpperInvariant());
            return new Number(num, _util);
        }

        public bool TryParse(string str, out Number number, string territoryName = null)
        {
            try
            {
                var phoneNumber = _util.Parse(str, (territoryName ?? DefaultTerritoryName)?.ToUpperInvariant());
                if (!_util.IsValidNumber(phoneNumber))
                {
                    number = null;
                    return false;
                }
                number = new Number(phoneNumber, _util);
                return true;
            }
            catch (NumberParseException)
            {
                number = null;
                return false;
            }
        }

        public string Normalize(string str, string territoryName = null)
        {
            var number = _util.Parse(str, (territoryName ?? DefaultTerritoryName)?.ToUpperInvariant());
            return _util.Format(number, PhoneNumberFormat.E164);
        }

        public bool TryNormalize(string str, out string number, string territoryName = null)
        {
            try
            {
                var num = _util.Parse(str, (territoryName ?? DefaultTerritoryName)?.ToUpperInvariant());
                number = _util.Format(num, PhoneNumberFormat.E164);
                return true;
            }
            catch (NumberParseException)
            {
                number = null;
                return false;
            }
        }

        public bool Validate(string str, string territoryName = null)
        {
            try
            {
                var num = _util.Parse(str, (territoryName ?? DefaultTerritoryName)?.ToUpperInvariant());
                var verify = _util.Verify(PhoneNumberUtil.Leniency.VALID, num, str, _util);
                return verify;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}