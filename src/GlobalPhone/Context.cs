using Makrill;
using PhoneNumbers;

namespace GlobalPhone
{
    public class Context
    {
        private PhoneNumberUtil _util;

        public Context()
        {
            _util = PhoneNumberUtil.GetInstance();
        }

        public PhoneNumber Parse(string str, string territoryName = null)
        {
            var num = _util.Parse(str, territoryName);
            return num;
        }

        public bool TryParse(string str, out PhoneNumber number, string territoryName = null)
        {
            number = _util.Parse(str, territoryName);

            var verify = _util.Verify(PhoneNumberUtil.Leniency.VALID, number, str, _util);
            if (!verify) number = null;
            return verify;
        }

        public string Normalize(string str, string territoryName = null)
        {
            var number = _util.Parse(str, territoryName);
            return _util.Format(number, PhoneNumberFormat.INTERNATIONAL);
        }

        public bool TryNormalize(string str, out string number, string territoryName = null)
        {
            var num = _util.Parse(str, territoryName);
            number = _util.Format(num, PhoneNumberFormat.INTERNATIONAL);
            return true;
        }

        public bool Validate(string str, string territoryName = null)
        {
            var num = _util.Parse(str, territoryName);
            var verify = _util.Verify(PhoneNumberUtil.Leniency.VALID, num, str, _util);
            return verify;
        }
    }
}