using System;
#if NEWTONSOFT
using Makrill;
using Newtonsoft.Json.Linq;
#else
using System.Web.Script.Serialization;
#endif

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
#if NEWTONSOFT
            private static readonly JsonConvert jsonConvert = new JsonConvert();
#else
            private static readonly JavaScriptSerializer jsonConvert = new JavaScriptSerializer();
#endif

            private static string SerializeObject(Object o)
            {
#if NEWTONSOFT
                return jsonConvert.Serialize(o);
#else
                return jsonConvert.Serialize(o);
#endif
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