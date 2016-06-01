using System;
using System.Collections;
using System.Collections.Generic;

namespace GlobalPhone
{
    internal static class InternalExtensions
    {
        internal static string ThrowIfNullOrEmpty<T>(this string obj, T exc) where T : Exception
        {
            if (String.IsNullOrEmpty(obj))
                throw exc;
            return obj;
        }
        internal static TRet ThrowIfNull<T, TRet>(this TRet obj, T exc)
            where T : Exception
            where TRet : class
        {
            if (obj == null)
                throw exc;
            return obj;
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
        internal static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TValue> getvalue)
        {
            if (!self.ContainsKey(key))
            {
                self[key] = getvalue();
            }
            return self[key];
        }

        public static TRet SelectWhereNotNull<T, TRet>(this IEnumerable<T> collection, Func<T, TRet> func) where TRet : class
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
    }
}
