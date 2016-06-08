using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHashV3)]
    public class NationalNumberTests<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public NationalNumberTests(ForData forData)
            : base(forData)
        { }

        [Test,
            TestCase("3125551212", "(312) 555-1212"),
            TestCase("02070313000", "+44 (0) 20-7031-3000"),
            TestCase("0771793336", "+46 771 793 336")]
        public void invalid_number(string expected, string number)
        {
            Assert.AreEqual(expected, 
                Context.Parse(number).NationalNumber);
        }
    }
}
