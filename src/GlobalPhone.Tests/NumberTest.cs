using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class NumberTest : TestFixtureBase
    {
        public NumberTest()
        {
            Context.DefaultTerritoryName = "us";
        }
        [Test]
        public void valid_number()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.IsValid);
        }
        [Test]
        public void equality_of_valid_number()
        {
            const string raw = "(312) 555-1212";
            var number = Context.Parse(raw);
            Assert.That(number == Context.Parse(raw), "==");
            Assert.That(number, Is.EqualTo( Context.Parse(raw)) ,"EqualTo");
        }
        [Test]
        public void invalid_number()
        {
            var number = Context.Parse("555-1212");
            Assert.That(!number.IsValid);
        }
        [Test]
        public void equality_of_invalid_number()
        {
            const string raw = "555-1212";
            var number = Context.Parse(raw);
            Assert.That(number == Context.Parse(raw), "==");
            Assert.That(number, Is.EqualTo( Context.Parse(raw)) ,"EqualTo");
        }
        [Test]
        public void inequality_of_numbers_from_different_territory()
        {
            const string raw = "(312) 555-1212";
            var number = Context.Parse(raw, "us");
            Assert.That(number != Context.Parse(raw, "se"), "==");
            Assert.That(number, Is.Not.EqualTo( Context.Parse(raw, "se")) ,"EqualTo");
        }
        [Test,
            TestCase(1, "(312) 555-1212"),
            TestCase(44, "+44 (0) 20-7031-3000"),
            ]
        public void country_code(int expected, string number)
        {
            Assert.That(Context.Parse(number).CountryCode, Is.EqualTo(expected));
        }

        [Test,
            TestCase("(312) 555-1212", "3125551212"),
            TestCase("+46 771 793 336", "0771793336")
        ]
        public void national_string(string raw, string expected)
        {
            Assert.That(Context.Parse(raw).NationalString, Is.EqualTo(expected));
        }
        [Test,
            TestCase("(312) 555-1212", "312-555-1212"),
            TestCase("077-179 33 36", "+46 771 793 336")
        ]
        public void national_format(string expected, string raw)
        {
            Assert.That(Context.Parse(raw).NationalFormat, Is.EqualTo(expected));
        }

        [Test]
        public void national_format_gb()
        {
            var number = Context.Parse("07411 111111", "gb");
            Assert.That(number.NationalFormat, Is.EqualTo("07411 111111"));
        }

        [Test,
            TestCase("+13125551212", "(312) 555-1212"),
            TestCase("+46771793336", "+46 771 793 336")
        ]
        public void international_string(string expected, string raw)
        {
            var number = Context.Parse(raw);
            Assert.That(number.InternationalString, Is.EqualTo(expected));
            Assert.That(number.ToString(), Is.EqualTo(number.InternationalString));
        }
        [Test,
            TestCase("+1 312-555-1212", "(312) 555-1212"),
            TestCase("+46 77 179 33 36", "+46771793336")
        ]
        public void international_format(string expected, string number)
        {
            Assert.That(Context.Parse(number).InternationalFormat, Is.EqualTo(expected));
        }

        [Test,
            TestCase("+61 3 9876 0010", "03"),
            TestCase("+44 (0) 20-7031-3000", "020"),
            TestCase("+852 2699 2838", ""),// Hong Kong has no area code
            TestCase("+1 801-710-1234", "801"),
            TestCase("+1 702-389-1234", "702"),
            ]
        public void area_code(string rawNumber, string areaCode)
        {
            var number = Context.Parse(rawNumber, "US");
            Assert.AreEqual(areaCode, number.AreaCode);
        }
        [Test, Ignore("good enough"),
            TestCase("+61 3 9876 0010", "9876 0010"),
            TestCase("+44 (0) 20-7031-3000", "7031 3000"),
            TestCase("+852 2699 2838", "2699 2838"),
            TestCase("+46 771 793 336", "79 33 36"),
        ]
        public void local_number(string rawNumber, string localNumber)
        {
            var number = Context.Parse(rawNumber);
            Assert.AreEqual(localNumber, number.LocalNumber);
        }

        [Test,
           TestCase("+61 3 9876 0010", "(03) 9876 0010"),
           TestCase("+44 (0) 20-7031-3000", "020 7031 3000"),
           TestCase("+852 2699 2838", "2699 2838"),
           TestCase("+46 771 793 336", "077-179 33 36"),
           TestCase("+1 650-253-0000", "(650) 253-0000")
       ]
        public void formatted_national_string(string rawNumber, string expected)
        {
            var number = Context.Parse(rawNumber);
            Assert.AreEqual(expected, number.FormattedNationalString);
        }

        [Test,
            TestCase("US_NUMBER", "+16502530000", "(650) 253-0000", "+1 650-253-0000"),
            TestCase("US_TOLLFREE", "+18002530000", "(800) 253-0000", "+1 800-253-0000"),
            TestCase("US_PREMIUM", "+19002530000", "(900) 253-0000", "+1 900-253-0000"),
        ]
        public void testFormatUSNumber(string name, string number,
            string nationalFormat, string internNationalFormat)
        {
            var parsedNumber = Context.Parse(number);
            Assert.AreEqual(nationalFormat,
                         parsedNumber.NationalFormat);
            Assert.AreEqual(internNationalFormat,
                         parsedNumber.InternationalFormat);
        }
    }
}
