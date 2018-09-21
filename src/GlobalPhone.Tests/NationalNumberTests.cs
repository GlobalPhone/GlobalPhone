using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class NationalNumberTests: TestFixtureBase 
    {
        [Test,
            TestCase(3125551212UL, "(312) 555-1212"),
            TestCase(2070313000UL, "+44 (0) 20-7031-3000"),
            TestCase(771793336UL, "+46 771 793 336")]
        public void invalid_number(ulong expected, string number)
        {
            Assert.AreEqual(expected, 
                Context.Parse(number, "US").NationalNumber);
        }
    }
}
