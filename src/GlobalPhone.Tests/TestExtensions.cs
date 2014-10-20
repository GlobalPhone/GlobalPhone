using System.Collections.Generic;
using System.Linq;
using System;

namespace GlobalPhone.Tests
{
    static class TestExtensions
    {
        internal static object DeleteOrUseDefault(this IDictionary<string,object> self,string key, object deflt )
        {
            if (self.ContainsKey(key))
            {
                var o = self[key];
                self.Remove(key);
                return o;
            }
            return deflt;
        }

		internal static IEnumerable<TRet> Map<T, TRet>(this IEnumerable<T> self, Func<T, TRet> map)
		{
			return self.Select(map);
		}
    }
}