using NUnit.Framework;
using System.Collections;
using System.Linq;
namespace GlobalPhone.Tests
{
	[TestFixture(typeof(DefaultDeserializer))]
	[TestFixture(typeof(NewtonsoftDeserializer))]
	public class SmokeTest<Deserializer> : TestFixtureBase where Deserializer:IDeserializer, new()
    {
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_deserializer = new Deserializer ();
		}

        [Test]
        public void parsing_example_numbers()
        {
            foreach (object obj in ExampleNumbers)
            {
				var item = ((IEnumerable)obj).Cast<object>().ToArray();
                var @string = item[0];
                var territory_name = item[1];
                assert_parses(@string, territory_name);
            }
        }

        private void assert_parses(object @string, object territory_name)
        {
            var number = Context.Parse((string)@string, (string)territory_name);
            Assert.That(number, Is.TypeOf<Number>(), "expected " + @string + " to parse for territory " + territory_name);
            Assert.NotNull(number.NationalString);
            Assert.NotNull(number.NationalFormat);
            Assert.NotNull(number.InternationalString);
            Assert.NotNull(number.InternationalFormat);
        }

        private void assert_can_handle_invalid(object @string, object territory_name)
        {
            var number = Context.TryParse((string)@string, (string)territory_name);
            Assert.That(number, Is.Null, "expected " + @string + " to fail to parse for territory " + territory_name);
            var normalized = Context.TryNormalize((string)@string, (string)territory_name);
            Assert.That(normalized, Is.Null, "expected " + @string + " to fail to normalize for territory " + territory_name);
        }

        [Test]
        public void parsing_invalid_numbers()
        {
            foreach (object obj in ExampleInvalidNumbers)
            {
				var item = ((IEnumerable)obj).Cast<object>().ToArray();

                var @string = item[0];
                var territory_name = item[1];
                assert_can_handle_invalid(@string, territory_name);
            }
        }
    }
}