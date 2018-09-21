using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace GlobalPhone
{
    class E161
    {

        private static readonly Dictionary<string, string> E161Mapping = "a2b2c2d3e3f3g4h4i4j5k5l5m6n6o6p7q7r7s7t8u8v8w9x9y9z9".SplitOnLength(2).ToDictionary(kv => kv[0].ToString(), kv => kv[1].ToString());
        private static readonly Regex ValidAlphaChars = new Regex("[a-zA-Z]", RegexOptions.Compiled);
        public static string Normalize(string str)
        {
            return ValidAlphaChars.Replace(str ?? String.Empty, match =>
                    E161Mapping[match.Value.ToLower()]);
        }
    }
}
