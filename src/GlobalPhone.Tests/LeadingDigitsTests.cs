using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class LeadingDigitsTests : TestFixtureBase 
    {
        public LeadingDigitsTests()
            : base(forData)
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
        }

        [Test, Ignore("Known bug")]
        public void formatting_of_gb_phone_number()
        {
            var number = Context.Parse("07400 123456", "GB");
            Assert.That(number.NationalFormat, Is.EqualTo("07400 123456"));
        }
    }
}
