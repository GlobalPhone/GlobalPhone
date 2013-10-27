using System;
using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class ContextTest : TestFixtureBase
    {
        private void assert_parses(string val, object obj)
        {
            var assertions = obj.ToHash();
            var territory_name = assertions.DeleteOrUseDefault("with_territory", context.default_territory_name);
            var number = context.parse(val, (string) territory_name);
            Assert.That(number,Is.TypeOf<Number>());
            Assert.That(new { country_code = number.country_code, national_string = number.national_string }.ToHash(), Is.EquivalentTo(assertions));
        }

        private void assert_does_not_parse(string val, object obj=null)
        {
            var assertions = ReflectionHelper.ToHash(obj);
            var territory_name = assertions.DeleteOrUseDefault("with_territory", context.default_territory_name);
            var number = context.parse(val, (string) territory_name);
            Assert.That(number,Is.Null,"expected " +val+ " not to parse for territory "+territory_name);
        }

        [Test]
        public void requires_db_path_to_be_set()
        {
            var context = new TestContext();
            Assert.Throws<NoDatabaseException>(() => { var _db = context.db; });
        }
        [Test]
        public void parsing_international_number()
        {
            assert_parses("+1-312-555-1212", new {country_code = "1", national_string = "3125551212"});
        }
        [Test]
        public void parsing_national_number_in_default_territory()
        {
            assert_parses("(312) 555-1212", new { country_code = "1", national_string = "3125551212" });
        }

        [Test]
        public void parsing_national_number_for_given_territory()
        {
            assert_parses("(0) 20-7031-3000", new { with_territory = "gb", country_code = "44", national_string = "2070313000" });
        }

        [Test]
        public void parsing_international_number_with_prefix()
        {
            assert_parses("00 1 3125551212", new { with_territory = "gb", country_code = "1", national_string = "3125551212" });
        }

        [Test]
        public void changing_the_default_territory()
        {
            assert_does_not_parse("(0) 20-7031-3000");

            context.default_territory_name = "gb";

            assert_parses("(0) 20-7031-3000", new { with_territory = "gb", country_code = "44", national_string = "2070313000" });
        }
        [Test]
        public void validating_an_international_number()
        {
            assert(context.validate("+1-312-555-1212"));
            assert(context.validate("+442070313000"));
            assert(!context.validate("+12345"));
        }
        [Test]
        public void validating_an_national_number()
        {
            assert(context.validate("312-555-1212"));
            assert(!context.validate("(0) 20-7031-3000"));
            assert(context.validate("(0) 20-7031-3000","gb"));
        }
        [Test]
        public void normalizing_an_international_number()
        {
            assert_equal("+13125551212", context.normalize("+1 312-555-1212"));
            assert_equal("+442070313000", context.normalize("+44 (0) 20-7031-3000"));
            assert_equal("+442070313000", context.normalize("+442070313000"));
            assert_nil(context.normalize("+12345"));
        }
        [Test]
        public void normalizing_a_national_number()
        {
            assert_equal("+13125551212", context.normalize("(312) 555-1212"));
            assert_nil(context.normalize("(0) 20-7031-3000"));
            assert_equal("+442070313000", context.normalize("(0) 20-7031-3000","gb"));
        }

    }
}
