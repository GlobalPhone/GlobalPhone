using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    internal static class Rubyfy
    {
        internal static string Unless<T>(this string obj, T exc) where T : Exception
        {
            if (string.IsNullOrEmpty(obj))
                throw exc;
            return obj;
        }
        internal static TRet Unless<T, TRet>(this TRet obj, T exc) where T : Exception
        {
            if (obj == null)
                throw exc;
            return obj;
        }

        internal static bool NotNull(this object self)
        {
            return self != null;
        }
        internal static T Tap<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
        internal static TResult Yield<T, TResult>(this T self, Func<T, TResult> action)
        {
            return action(self);
        }
        internal static string gsub(this string self, Regex regex, string evaluator)
        {
            if (evaluator.Contains("$"))
            {
                return regex.Replace(self, match =>
                                               {
                                                   var eval = evaluator;
                                                   for (int i = 1; i < match.Groups.Count; i++)
                                                   {
                                                       var g = match.Groups[i];
                                                       if (g.Success)
                                                       {
                                                           eval = eval.Replace("$" + (i), g.Value);
                                                       }
                                                   }
                                                   //var matches = new Regex("$\\d").Matches();
                                                   //($1) $2-$3
                                                   return eval;
                                               });
            }
            return regex.Replace(self, evaluator);
        }
        internal static string gsub(this string self, string regex, string evaluator)
        {
            return regex.Replace(self, evaluator);
        }
        internal static string sub(this string self, Regex regex, string evaluator)
        {
            return regex.Replace(self, evaluator, 1);
        }
        internal static string gsub(this string self, Regex regex, MatchEvaluator evaluator)
        {
            return regex.Replace(self, evaluator);
        }

        internal static Match match(this string self, Regex regex)
        {
            return regex.Match(self ?? string.Empty);
        }

        internal static IEnumerable<TRet> Map<T, TRet>(this IEnumerable<T> self, Func<T, TRet> map)
        {
            return self.Select(map);
        }
        internal static string[] SplitN(this string self, int num)
        {
            var result = new string[self.Length / num];
            var index = 0;
            for (int i = 0; i < self.Length; i += num)
            {
                result[index++] = self.Substring(i, num);
            }
            return result;
        }
        internal static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TValue> getvalue)
        {
            if (!self.ContainsKey(key))
            {
                self[key] = getvalue();
            }
            return self[key];
        }
        internal static T detect<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            return self.FirstOrDefault(predicate);
        }

    }
}
