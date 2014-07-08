using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class EdgeTest : TestFixtureBase
    {
        [Test]
        public void formatting_numbers_that_match_a_pattern_but_not_leading_digits()
        {
            /*  
          # Because of the way we build our database, some numbers
          # will match a format pattern but not its leading digits
          # regex. Instead of requiring a number's format to match
          # both the format pattern and leading digits, we will
          # prefer the format whose leading digits match, if possible,
          # or fall back to the first pattern-matched format.

          # In this case, 1520123456 matches one of Ireland's "premium
          # rate" format specifications in PhoneNumberMetaData.xml.
          # We don't include those formats in our database, so we fall
          # back to the closest match.
             */
            var number = Context.Parse("1520123456", "IE");
            Assert.That(number.NationalFormat, Is.EqualTo("1520 123 456"));
        }

    }
}