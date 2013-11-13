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
            public JsonEqualConstraint(object match)
                :base(Newtonsoft.Json.JsonConvert.SerializeObject(match))
            {
            }

            public override bool Matches(object actual)
            {
                return base.Matches(Newtonsoft.Json.JsonConvert.SerializeObject(actual));
            }
        }
        public static IResolveConstraint EqualTo(object match)
        {
            return new JsonResolveConstraint(match);
        }
    }
}