using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class ContextTest : TestFixtureBase
    {
        private void assert_parses(string val, object obj)
        {
            var assertions = obj.ToHash();
            var territory_name = assertions.DeleteOrUseDefault("with_territory", Context.DefaultTerritoryName);
            var number = Context.Parse(val, (string) territory_name);
            NUnit.Framework.Assert.That(number,Is.TypeOf<Number>());
            NUnit.Framework.Assert.That(new { country_code = number.CountryCode, national_string = number.NationalString }.ToHash(), Is.EquivalentTo(assertions));
        }

        private void assert_does_not_parse(string val, object obj=null)
        {
            var assertions = obj.ToHash();
            var territoryName = assertions.DeleteOrUseDefault("with_territory", Context.DefaultTerritoryName);
            var number = Context.Parse(val, (string) territoryName);
            NUnit.Framework.Assert.That(number,Is.Null,"expected " +val+ " not to parse for territory "+territoryName);
        }

        [Test]
        public void requires_db_path_to_be_set()
        {
            var context = new Context();
            NUnit.Framework.Assert.Throws<NoDatabaseException>(() => { var _db = context.Db; });
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

            Context.DefaultTerritoryName = "gb";

            assert_parses("(0) 20-7031-3000", new { with_territory = "gb", country_code = "44", national_string = "2070313000" });
        }
        [Test]
        public void validating_an_international_number()
        {
            Assert(Context.Validate("+1-312-555-1212"));
            Assert(Context.Validate("+442070313000"));
            Assert(!Context.Validate("+12345"));
        }
        [Test]
        public void validating_an_national_number()
        {
            Assert(Context.Validate("312-555-1212"));
            Assert(!Context.Validate("(0) 20-7031-3000"));
            Assert(Context.Validate("(0) 20-7031-3000","gb"));
        }
        [Test]
        public void normalizing_an_international_number()
        {
            assert_equal("+13125551212", Context.Normalize("+1 312-555-1212"));
            assert_equal("+442070313000", Context.Normalize("+44 (0) 20-7031-3000"));
            assert_equal("+442070313000", Context.Normalize("+442070313000"));
            assert_nil(Context.Normalize("+12345"));
        }
        [Test]
        public void normalizing_a_national_number()
        {
            assert_equal("+13125551212", Context.Normalize("(312) 555-1212"));
            assert_nil(Context.Normalize("(0) 20-7031-3000"));
            assert_equal("+442070313000", Context.Normalize("(0) 20-7031-3000","gb"));
        }

    }
}
