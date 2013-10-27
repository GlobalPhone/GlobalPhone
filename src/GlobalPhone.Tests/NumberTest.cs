using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class NumberTest : TestFixtureBase
    {
        [Test]
        public void valid_number()
        {
            var number = context.parse("(312) 555-1212");
            assert(number.valid());
        }
        [Test]
        public void invalid_number()
        {
            var number = context.parse("555-1212");
            assert(!number.valid());
        }
        [Test]
        public void country_code()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal("1", number.country_code);

            number = context.parse("+44 (0) 20-7031-3000");
            assert_equal("44", number.country_code);
        }
        [Test]
        public void region()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal(db.region(1), number.region);

            number = context.parse("+44 (0) 20-7031-3000");
            assert_equal(db.region(44), number.region);
        }
        [Test]
        public void territory()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal(db.territory("us"), number.territory);

            number = context.parse("+44 (0) 20-7031-3000");
            assert_equal(db.territory("gb"), number.territory);
        }

        [Test]
        public void national_string()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal("3125551212", number.national_string);
        }
        [Test]
        public void national_format()
        {
            var number = context.parse("312-555-1212");
            assert_equal("(312) 555-1212", number.national_format);
        }
        [Test]
        public void international_string()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal("+13125551212", number.international_string);
            assert_equal(number.international_string, number.ToString());
        }
        [Test]
        public void international_format()
        {
            var number = context.parse("(312) 555-1212");
            assert_equal("+1 312-555-1212", number.international_format);
        }
    }
}