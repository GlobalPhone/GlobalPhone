using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace GlobalPhone
{
    class E161
    {
        //http://en.wikipedia.org/wiki/E.161
        public static bool UsedBy(Territory territory)
        {
            return Nanp.Countries.Contains(territory.Name);
        }

        private static readonly Dictionary<string, string> E161Mapping = "a2b2c2d3e3f3g4h4i4j5k5l5m6n6o6p7q7r7s7t8u8v8w9x9y9z9".SplitOnLength(2).ToDictionary(kv => kv[0].ToString(CultureInfo.InvariantCulture), kv => kv[1].ToString(CultureInfo.InvariantCulture));
        private static readonly Regex ValidAlphaChars = new Regex("[a-zA-Z]", RegexOptions.Compiled);
        public static string Normalize(string str)
        {
            return ValidAlphaChars.Replace(str ?? String.Empty, match =>
                    E161Mapping[match.Value.ToLower()]);
        }
    }
}
