using System.IO;
using JsonConvert = Makrill.JsonConvert;

namespace GlobalPhone.Tests
{
    public class TestFixtureBase
    {
        public readonly Context Context = new Context();

        private object[] _exampleNumbers;
        private object[] _globalPhoneTestCases;
        private object[] _exampleInvalidNumbers;
        private static readonly JsonConvert _jsonConvert = new Makrill.JsonConvert();

        public object[] ExampleInvalidNumbers
        {
            get
            {
                return _exampleInvalidNumbers ?? (_exampleInvalidNumbers = new object[]
                {
                    new[] {"689", "AC"},
                    new[] {"112", "SE"},
                    new[] {"000", "SE"},
                    new[] {"ABC", "SE"},
                });
            }
        }

        private static string FixturePath(string file)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(TestFixtureBase).Assembly.Location),
                "fixtures", file);
        }

        private object[] JsonFixture(string name)
        {
            return _jsonConvert.Deserialize<object[]>(File.ReadAllText(FixturePath(name + ".json")));
        }

        private string GetExampleNumbersFixtureName()
        {
            // use latest
            return "example_numbers_v2";
        }

        public object[] ExampleNumbers
        {
            get { return _exampleNumbers ?? (_exampleNumbers = JsonFixture(GetExampleNumbersFixtureName())); }
        }

        public object[] GlobalPhoneTestCases
        {
            get { return _globalPhoneTestCases ?? (_globalPhoneTestCases = JsonFixture("global_phone_test_cases")); }
        }
    }
}