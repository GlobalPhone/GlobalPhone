using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV2)]
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHashV2)]
    public class NumberTest<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public NumberTest(ForData forData)
            : base(forData)
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
        }
        [Test]
        public void valid_number()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.IsValid);
        }
        [Test]
        public void invalid_number()
        {
            var number = Context.Parse("555-1212");
            Assert.That(!number.IsValid);
        }
        [Test]
        public void country_code()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.CountryCode, Is.EqualTo("1"));

            number = Context.Parse("+44 (0) 20-7031-3000");
            Assert.That(number.CountryCode, Is.EqualTo("44"));
        }
        [Test]
        public void region()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.Region, Is.EqualTo(Db.TryGetRegion(1)));

            number = Context.Parse("+44 (0) 20-7031-3000");
            Assert.That(number.Region, Is.EqualTo(Db.TryGetRegion(44)));
        }
        [Test]
        public void territory()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.Territory, Is.EqualTo(Db.TryGetTerritory("us")));

            number = Context.Parse("+44 (0) 20-7031-3000");
            Assert.That(number.Territory, Is.EqualTo(Db.TryGetTerritory("gb")));
        }

        [Test]
        public void national_string()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.NationalString, Is.EqualTo("3125551212"));
        }
        [Test]
        public void national_format()
        {
            var number = Context.Parse("312-555-1212");
            Assert.That(number.NationalFormat, Is.EqualTo("(312) 555-1212"));
        }

        [Test]
        public void national_format_gb()
        {
            var number = Context.Parse("07411 111111", "gb");
            Assert.That(number.NationalFormat, Is.EqualTo("07411 111111"), "for data "+_forData);
        }

        [Test]
        public void international_string()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.InternationalString, Is.EqualTo("+13125551212"));
            Assert.That(number.ToString(), Is.EqualTo(number.InternationalString));
        }
        [Test]
        public void international_format()
        {
            var number = Context.Parse("(312) 555-1212");
            Assert.That(number.InternationalFormat, Is.EqualTo("+1 312-555-1212"));
        }

        [Test, 
            TestCase("+61 3 9876 0010", "03"),
            TestCase("+44 (0) 20-7031-3000", "020"),
            TestCase("+852 2699 2838", null),// Hong Kong has no area code
            TestCase("+1 801-710-1234", "801"),
            TestCase("+1 702-389-1234", "702"),
            ]
        public void area_code(string rawNumber, string areaCode)
        {
            var number = Context.Parse(rawNumber);
            Assert.AreEqual(areaCode, number.AreaCode);
        }
        [Test, 
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