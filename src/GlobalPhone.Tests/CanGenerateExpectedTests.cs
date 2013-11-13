using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class CanGenerateExpectedTests : TestFixtureBase
    {
        
        [Test]
        public void The_json_matches_what_ruby_generates()
        {
            var dbgen = DatabaseGenerator.Load(PhoneNumberMetadata);
            var recordData = dbgen.RecordData();
            Assert.That(recordData, Json.EqualTo(GlobalPhone));
        }

        [Test]
        public void The_json_matches_what_ruby_generates_test_cases()
        {
            var dbgen = DatabaseGenerator.Load(PhoneNumberMetadata);
            var recordData = dbgen.TestCases();
            Assert.That(recordData, Json.EqualTo(GlobalPhoneTestCases));
        }
    
    }
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
            private readonly object _match;

            public JsonEqualConstraint(object match)
                :base(Newtonsoft.Json.JsonConvert.SerializeObject(match))
            {
                _match = match;
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
