using System;
using NUnit.Framework;
using PhoneNumbers;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class ContextTest : TestFixtureBase 
    {
        public ContextTest()
        {
            Context.DefaultTerritoryName = "us";
        }
        class Conf
        {
            public int country_code
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

        private void assert_parses(string val, Conf obj)
        {
            var assertions = obj.ToHash();
            var territoryName = assertions.DeleteOrUseDefault("with_territory", Context.DefaultTerritoryName);
            var number = Context.Parse(val, (string)territoryName);
            Assert.That(number, Is.TypeOf<Number>());
            Assert.That(new
            {
                country_code = number.CountryCode,
                national_string = number.NationalString
            }.ToHash(),
                Is.EquivalentTo(assertions));
        }

        [Test]
        public void parsing_international_number()
        {
            assert_parses("+1-312-555-1212", new Conf { country_code = 1, national_string = "3125551212" });
        }
        [Test]
        public void parsing_national_number_in_default_territory()
        {
            assert_parses("(312) 555-1212", new Conf { country_code = 1, national_string = "3125551212" });
        }

        [Test]
        public void parsing_national_number_for_given_territory()
        {
            assert_parses("(0) 20-7031-3000", new Conf { with_territory = "gb", country_code = 44, national_string = "02070313000" });
        }

        [Test]
        public void parsing_international_number_with_prefix()
        {
            assert_parses("00 1 3125551212", new Conf { with_territory = "gb", country_code = 1, national_string = "3125551212" });
        }

        [Test]
        public void try_parse_with_illegal_territory_should_not_throw()
        {
            Assert.DoesNotThrow(() => Context.TryParse("12345", out _, "alderaan"));
        }

        [Test]
        public void changing_the_default_territory()
        {
            assert_parses("(0) 20-7031-3000", new Conf { with_territory = "gb", country_code = 44, national_string = "02070313000" });
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
            Assert.That(Context.Validate("312-555-1212", "us"));
            Assert.That(!Context.Validate("(0) 20-7031-3000", "us"));
            Assert.That(Context.Validate("(0) 20-7031-3000", "gb"));
        }
        [Test]
        public void normalizing_an_international_number()
        {
            Assert.That(Context.Normalize("+1 312-555-1212"), Is.EqualTo("+13125551212"));
            Assert.That(Context.Normalize("+44 (0) 20-7031-3000"), Is.EqualTo("+442070313000"));
            Assert.That(Context.Normalize("+442070313000"), Is.EqualTo("+442070313000"));
            //Assert.Throws<FailedToParseNumberException>(() => Context.Normalize("+12345"));
        }
        [Test]
        public void normalizing_a_national_number()
        {
            Assert.That(Context.Normalize("(312) 555-1212", "us"), Is.EqualTo("+13125551212"));
            //Assert.Throws<NumberParseException>(() => Context.Normalize("(0) 20-7031-3000", "us"));
            Assert.That(Context.Normalize("(0) 20-7031-3000", "gb"), Is.EqualTo("+442070313000"));
        }
        [Test]
        public void try_normalize_with_illegal_territory_should_not_throw()
        {
            Assert.DoesNotThrow(() => Context.TryNormalize("12345", out _, "alderaan"));
        }
        [Test]
        public void try_normalize_with_empty_or_null_should_not_throw()
        {
            Assert.DoesNotThrow(() => Context.TryNormalize("", out _));
            Assert.DoesNotThrow(() => Context.TryNormalize(null, out _));
        }
        [Test]
        public void try_parse_with_empty_or_null_should_not_throw()
        {
            Assert.DoesNotThrow(() => Context.TryParse("", out _));
            Assert.DoesNotThrow(() => Context.TryParse(null, out _));
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
        [Test, Ignore("Does not work")]
        public void should_not_alpha_numeric_to_number_when_in_se()
        {
            Assert.Throws<NumberParseException>(() => Context.Normalize("1800COMPUTE", "se"));
        }
    }
}
