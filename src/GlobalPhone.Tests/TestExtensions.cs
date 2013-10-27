using System.Collections.Generic;

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
    }
}