using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class NumberTest : TestFixtureBase
    {
        [Test]
        public void valid_number()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert(number.IsValid);
        }
        [Test]
        public void invalid_number()
        {
            var number = Context.Parse("555-1212");
            Assert(!number.IsValid);
        }
        [Test]
        public void country_code()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal("1", number.CountryCode);

            number = Context.Parse("+44 (0) 20-7031-3000");
            assert_equal("44", number.CountryCode);
        }
        [Test]
        public void region()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal(Db.Region(1), number.Region);

            number = Context.Parse("+44 (0) 20-7031-3000");
            assert_equal(Db.Region(44), number.Region);
        }
        [Test]
        public void territory()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal(Db.Territory("us"), number.Territory);

            number = Context.Parse("+44 (0) 20-7031-3000");
            assert_equal(Db.Territory("gb"), number.Territory);
        }

        [Test]
        public void national_string()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal("3125551212", number.NationalString);
        }
        [Test]
        public void national_format()
        {
            var number = Context.Parse("312-555-1212");
            assert_equal("(312) 555-1212", number.NationalFormat);
        }
        [Test]
        public void international_string()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal("+13125551212", number.InternationalString);
            assert_equal(number.InternationalString, number.ToString());
        }
        [Test]
        public void international_format()
        {
            var number = Context.Parse("(312) 555-1212");
            assert_equal("+1 312-555-1212", number.InternationalFormat);
        }
    }
}