using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GlobalPhone.Tests
{
    static class ReflectionHelper
    {
        public static IDictionary<string, object> ToHash(this object o)
        {
            if (null==o)
            {
                return new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            }
            return o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(o, null), StringComparer.InvariantCultureIgnoreCase);
        }
    }
}