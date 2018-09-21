using System;
using System.Collections;

namespace GlobalPhone
{
    internal static class InternalExtensions
    {
        internal static TResult Yield<T, TResult>(this T self, Func<T, TResult> action)
        {
            return action(self);
        }

        /// <summary>
        /// split the string into length large pieces
        /// </summary>
        /// <param name="self"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static string[] SplitOnLength(this string self, int length)
        {
            var result = new string[self.Length / length];
            var index = 0;
            for (int i = 0; i < self.Length; i += length)
            {
                result[index++] = self.Substring(i, length);
            }
            return result;
        }
    }
}
