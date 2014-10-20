using System;
using Makrill;
using Newtonsoft.Json.Linq;

using NUnit.Framework.Constraints;

namespace GlobalPhone.Tests
{
    public class Json
    {
        private class JsonResolveConstraint : IResolveConstraint 
        {
            private readonly object _match;

            public JsonResolveConstraint(object match)
            {
                _match = match;
            }

            public Constraint Resolve()
            {
                return new JsonEqualConstraint(_match);
            }
        }
        internal class JsonEqualConstraint :EqualConstraint
        {
            private static readonly JsonConvert jsonConvert = new JsonConvert();

            private static string SerializeObject(Object o)
            {
                return jsonConvert.Serialize(o);
            }

            public JsonEqualConstraint(object match)
                :base(SerializeObject(match))
            {
            }

            public override bool Matches(object actual)
            {
                return base.Matches(SerializeObject(actual));
            }
        }
        public static IResolveConstraint EqualTo(object match)
        {
            return new JsonResolveConstraint(match);
        }
    }
}