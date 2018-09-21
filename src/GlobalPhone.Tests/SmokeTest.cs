using System;
using NUnit.Framework;
using System.Collections;
using System.Linq;
namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHash)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV2)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV3)]
    public class SmokeTest<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public SmokeTest(ForData forData)
            : base(forData)
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
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
            Number number = null;
            try
            {
                number = Context.Parse((string)@string, (string)territory_name);
            }
            catch (Exception ex)
            {
                throw new Exception(Message(@string, territory_name), ex);
            }
            Assert.That(number, Is.TypeOf<Number>(), Message(@string, territory_name));
            Assert.NotNull(number.NationalString);
            Assert.NotNull(number.NationalFormat);
            Assert.NotNull(number.InternationalString);
            Assert.NotNull(number.InternationalFormat);
        }

        private string Message(object @string, object territoryName)
        {
            return "expected " + @string + " to parse for territory " + territoryName + " for data "+_forData;
        }

        private void assert_can_handle_invalid(object @string, object territory_name)
        {
            Number number;
            Context.TryParse((string)@string, out number, (string)territory_name);
            Assert.That(number, Is.Null, "expected " + @string + " to fail to parse for territory " + territory_name);
            string normalized;
            Context.TryNormalize((string)@string, out normalized, (string)territory_name);
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