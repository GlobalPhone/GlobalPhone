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
            Assert.That(number.NationalFormat, Is.EqualTo("07411 111111"));
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
    }
}