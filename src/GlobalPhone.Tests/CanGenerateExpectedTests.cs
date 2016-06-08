using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHash)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV2)]
    public class CanGenerateExpectedTests<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public CanGenerateExpectedTests(ForData forData)
            : base(forData)
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
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
