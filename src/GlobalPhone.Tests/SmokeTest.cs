using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class SmokeTest : TestFixtureBase
    {
        [Test]
        public void parsing_example_numbers()
        {
            foreach (object[] item in example_numbers)
            {
                var @string = item[0];
                var territory_name = item[1];
                assert_parses(@string, territory_name);
            }
        }

        private void assert_parses(object @string, object territory_name)
        {
            var number = context.parse((string)@string, (string)territory_name);
          Assert.That(number,Is.TypeOf<Number>(),"expected "+@string+" to parse for territory "+territory_name);
          Assert.NotNull( number.national_string);
          Assert.NotNull( number.national_format);
          Assert.NotNull( number.international_string);
          Assert.NotNull( number.international_format);
        }
    }
}