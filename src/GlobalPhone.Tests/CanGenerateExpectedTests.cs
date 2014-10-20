using NUnit.Framework;

namespace GlobalPhone.Tests
{
	[TestFixture(typeof(DefaultDeserializer))]
	[TestFixture(typeof(NewtonsoftDeserializer))]
	public class CanGenerateExpectedTests<Deserializer> : TestFixtureBase where Deserializer:IDeserializer, new()
    {
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_deserializer = new Deserializer ();
		}

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
