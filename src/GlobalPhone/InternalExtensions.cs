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
    }
}
