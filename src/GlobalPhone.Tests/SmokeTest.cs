using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class SmokeTest : TestFixtureBase
    {
        [Test]
        public void parsing_example_numbers()
        {
            foreach (object[] item in ExampleNumbers)
            {
                var @string = item[0];
                var territory_name = item[1];
                assert_parses(@string, territory_name);
            }
        }

        private void assert_parses(object @string, object territory_name)
        {
            var number = Context.Parse((string)@string, (string)territory_name);
          NUnit.Framework.Assert.That(number,Is.TypeOf<Number>(),"expected "+@string+" to parse for territory "+territory_name);
          NUnit.Framework.Assert.NotNull( number.NationalString);
          NUnit.Framework.Assert.NotNull( number.NationalFormat);
          NUnit.Framework.Assert.NotNull( number.InternationalString);
          NUnit.Framework.Assert.NotNull( number.InternationalFormat);
        }
    }
}