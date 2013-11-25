using NUnit.Framework;

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
        public void The_json_matches_what_ruby_generates2()
        {
            var dbgen = DatabaseGenerator.Load(PhoneNumberMetadata2);
            var recordData = dbgen.RecordData();
            Assert.That(recordData, Json.EqualTo(GlobalPhone2));
        }

        [Test]
        public void The_json_matches_what_ruby_generates_test_cases()
        {
            var dbgen = DatabaseGenerator.Load(PhoneNumberMetadata);
            var recordData = dbgen.TestCases();
            Assert.That(recordData, Json.EqualTo(GlobalPhoneTestCases));
        }
    
    }
}
