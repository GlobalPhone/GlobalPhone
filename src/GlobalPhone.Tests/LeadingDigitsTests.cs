using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class LeadingDigitsTests : TestFixtureBase 
    {
        [Test]
        public void formatting_of_gb_phone_number()
        {
            var number = Context.Parse("07400 123456", "GB");
            Assert.That(number.NationalNumber, Is.EqualTo(07400_123456));
        }
    }
}
