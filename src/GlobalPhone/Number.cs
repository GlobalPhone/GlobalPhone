using System;
using System.Linq;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace GlobalPhone
{
    public static class Number
    {
       

        private static readonly Regex LeadingPlusChars = new Regex("^\\++", RegexOptions.Compiled);
        private static readonly Regex NonDialableChars = new Regex("[^,#+\\*\\d]", RegexOptions.Compiled);
        /// <summary>
        /// Normalize the specified str based on territory.
        /// if null is sent in as territory, assumes that E161 is not used.
        /// </summary>
        /// <param name="str">a number</param>
        /// <param name="territory">Territory to identify if you should use E161.</param>
         internal static string Normalize(PhoneNumbers.PhoneNumber str)
        {
            
            
            return (str.CountryCode == 1
                    ? E161.Normalize(str)
                    : str)
                    .Yield(s => LeadingPlusChars.Replace(s, "+"))
                    .Yield(s => NonDialableChars.Replace(s, ""));
        }
       
    }
}
