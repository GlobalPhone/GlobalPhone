using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalPhone
{
    internal static class Rubyfy
    {
        internal static string Unless<T>(this string obj, T exc) where T : Exception
        {
            if (String.IsNullOrEmpty(obj))
                throw exc;
            return obj;
        }
        internal static TRet Unless<T, TRet>(this TRet obj, T exc)
            where T : Exception
            where TRet : class
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
        internal static string Gsub(this string self, Regex regex, string evaluator)
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
                                                   return eval;
                                               });
            }
            return regex.Replace(self, evaluator);
        }
        internal static string Gsub(this string self, string regex, string evaluator)
        {
            return new Regex(regex).Replace(self ?? String.Empty, evaluator);
        }
        internal static string Sub(this string self, Regex regex, string evaluator)
        {
            return regex.Replace(self ?? String.Empty, evaluator, 1);
        }
        internal static string Gsub(this string self, Regex regex, MatchEvaluator evaluator)
        {
            return regex.Replace(self ?? String.Empty, evaluator);
        }

        internal static Match Match(this string self, Regex regex)
        {
            return regex.Match(self ?? String.Empty);
        }

        internal static IEnumerable<TRet> Map<T, TRet>(this IEnumerable<T> self, Func<T, TRet> map)
        {
            return self.Select(map);
        }
        /// <summary>
        /// Returns a new array that is a one-dimensional flattening of self (recursively).
        ///
        ///That is, for every element that is an array, extract its elements into the new array.
        /// </summary>
        internal static IEnumerable<T> Flatten<T>(this IEnumerable self)
        {
            foreach (var variable in self)
            {
                if (variable is T)
                {
                    yield return (T)variable;
                }
                else
                {
                    foreach (var result in Flatten<T>((IEnumerable)variable))
                    {
                        yield return result;
                    }
                }
            }
        }
     
        /// <summary>
        /// Returns a new array that is a one-dimensional flattening of self (recursively).
        ///
        ///That is, for every element that is an array, extract its elements into the new array.
        ///
        ///The optional level argument determines the level of recursion to flatten.
        /// </summary>
        internal static IEnumerable Flatten(this IEnumerable self, int? order=null)
        {
            if (order==null || order >= 0)
            {
                foreach (var variable in self)
                {
                    if (variable is IEnumerable && !(variable is string))
                    {
                        foreach (var result in Flatten((IEnumerable)variable, order!=null? order - 1:null))
                        {
                            yield return result;
                        }
                    }
                    else
                    {
                        yield return variable;
                    }
                }
            }
            else
            {
                yield return self;
            }
        }

        internal static bool IsEmpty<T>(this IEnumerable<T> self)
        {
            return null == self || !self.Any();
        }

        /// <summary>
        /// split the string into length large pieces
        /// </summary>
        /// <param name="self"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static string[] SplitN(this string self, int length)
        {
            var result = new string[self.Length / length];
            var index = 0;
            for (int i = 0; i < self.Length; i += length)
            {
                result[index++] = self.Substring(i, length);
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
        internal static T Detect<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            return self.FirstOrDefault(predicate);
        }

        public static TRet MapDetect<T, TRet>(this IEnumerable<T> collection, Func<T, TRet> func) where TRet : class
        {
            foreach (var value in collection)
            {
                TRet result;
                if ((result = func(value)) != null)
                {
                    return result;
                }
            }
            return null;
        }

        internal static T[] ToA<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToArray();
        }

        internal static object[] ToA(this IEnumerable enumerable)
        {
            return enumerable.Cast<Object>().ToArray();
        }
    }
}
