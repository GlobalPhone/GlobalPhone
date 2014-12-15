using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHash)]
    [TestFixture(typeof(DefaultDeserializer), ForData.UseArray)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseArray)]
    public class ContextTest<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public ContextTest(ForData forData)
            : base(forData)
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
        }
        class Conf
        {
            public string country_code
            {
                get;
                set;
            }
            public string national_string
            {
                get;
                set;
            }
            public string with_territory
            {
                get;
                set;
            }
        }
        [SetUp]
        public void SetUp()
        {
            Context.DefaultTerritoryName = "US";
        }

        private void assert_parses(string val, Conf obj)
        {
            var assertions = obj.ToHash();
            var territory_name = assertions.DeleteOrUseDefault("with_territory", Context.DefaultTerritoryName);
            var number = Context.Parse(val, (string)territory_name);
            Assert.That(number, Is.TypeOf<Number>());
            Assert.That(new
            {
                country_code = number.CountryCode,
                national_string = number.NationalString
            }.ToHash(),
                Is.EquivalentTo(assertions));
        }

        private void assert_does_not_parse(string val, Conf obj = null)
        {
            var assertions = obj.ToHash();
            var territoryName = assertions.DeleteOrUseDefault("with_territory", Context.DefaultTerritoryName);
            Assert.Throws<FailedToParseNumberException>(() => Context.Parse(val, (string)territoryName), "expected " + val + " not to parse for territory " + territoryName);
        }

        [Test]
        public void requires_db_path_to_be_set()
        {
            var context = new Context();
            Assert.Throws<NoDatabaseException>(() => { var _db = context.Db; });
        }
        [Test]
        public void parsing_international_number()
        {
            assert_parses("+1-312-555-1212", new Conf { country_code = "1", national_string = "3125551212" });
        }
        [Test]
        public void parsing_national_number_in_default_territory()
        {
            assert_parses("(312) 555-1212", new Conf { country_code = "1", national_string = "3125551212" });
        }

        [Test]
        public void parsing_national_number_for_given_territory()
        {
            assert_parses("(0) 20-7031-3000", new Conf { with_territory = "gb", country_code = "44", national_string = "2070313000" });
        }

        [Test]
        public void parsing_international_number_with_prefix()
        {
            assert_parses("00 1 3125551212", new Conf { with_territory = "gb", country_code = "1", national_string = "3125551212" });
        }

        [Test]
        public void try_parse_with_illegal_territory_should_not_throw()
        {
            Number number;
            Assert.DoesNotThrow(() => Context.TryParse("12345", out number, "alderaan"));
        }

        [Test]
        public void changing_the_default_territory()
        {
            assert_does_not_parse("(0) 20-7031-3000");

            Context.DefaultTerritoryName = "gb";

            assert_parses("(0) 20-7031-3000", new Conf { with_territory = "gb", country_code = "44", national_string = "2070313000" });
        }
        [Test]
        public void validating_an_international_number()
        {
            Assert.That(Context.Validate("+1-312-555-1212"));
            Assert.That(Context.Validate("+442070313000"));
            Assert.That(!Context.Validate("+12345"));
        }
        [Test]
        public void validating_an_national_number()
        {
            Assert.That(Context.Validate("312-555-1212"));
            Assert.That(!Context.Validate("(0) 20-7031-3000"));
            Assert.That(Context.Validate("(0) 20-7031-3000", "gb"));
        }
        [Test]
        public void normalizing_an_international_number()
        {
            Assert.That(Context.Normalize("+1 312-555-1212"), Is.EqualTo("+13125551212"));
            Assert.That(Context.Normalize("+44 (0) 20-7031-3000"), Is.EqualTo("+442070313000"));
            Assert.That(Context.Normalize("+442070313000"), Is.EqualTo("+442070313000"));
            Assert.Throws<FailedToParseNumberException>(() => Context.Normalize("+12345"));
        }
        [Test]
        public void normalizing_a_national_number()
        {
            Assert.That(Context.Normalize("(312) 555-1212"), Is.EqualTo("+13125551212"));
            Assert.Throws<FailedToParseNumberException>(() => Context.Normalize("(0) 20-7031-3000"));
            Assert.That(Context.Normalize("(0) 20-7031-3000", "gb"), Is.EqualTo("+442070313000"));
        }
        [Test]
        public void try_normalize_with_illegal_territory_should_not_throw()
        {
            string normalized;
            Assert.DoesNotThrow(() => Context.TryNormalize("12345", out normalized, "alderaan"));
        }
        [Test]
        public void try_normalize_with_empty_or_null_should_not_throw()
        {
            string normalized;
            Assert.DoesNotThrow(() => Context.TryNormalize("", out normalized));
            Assert.DoesNotThrow(() => Context.TryNormalize(null, out normalized));
        }
        [Test]
        public void try_parse_with_empty_or_null_should_not_throw()
        {
            Number parsed;
            Assert.DoesNotThrow(() => Context.TryParse("", out parsed));
            Assert.DoesNotThrow(() => Context.TryParse(null, out parsed));
        }
        [Test]
        public void should_parse_number_from_argentina()
        {
            Assert.AreEqual("+54 11 4799-9350", Context.Parse("+54 11 4799 9350").InternationalFormat);
        }
        [Test]
        public void should_normalize_alpha_numeric_to_number()
        {
            Assert.AreEqual("+18002667883", Context.Normalize("1800COMPUTE", "us"));
        }
        [Test]
        public void should_not_alpha_numeric_to_number_when_in_se()
        {
            Assert.Throws<FailedToParseNumberException>(() => Context.Normalize("1800COMPUTE", "se"));
        }
    }
}
