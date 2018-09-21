using System.IO;
using NUnit.Framework;
using System;
using Newtonsoft.Json;

namespace GlobalPhone.Tests
{
    public class TestFixtureBase
    {

        public readonly Context Context = new Context();


        private object[] _globalPhoneTestCases;
        private object[] _exampleInvalidNumbers;

        public object[] ExampleInvalidNumbers
        {
            get
            {
                return _exampleInvalidNumbers ?? (_exampleInvalidNumbers = new object[]
                {
                    new []{"689","AC"},
                    new []{"112","SE"},
                    new []{"000","SE"},
                    new []{"ABC","SE"},
                });
            }
        }
        private static string FixturePath(string file)
                {
                    return Path.Combine("fixtures", file);
                }
        private object[] JsonFixture(string name)
                    {
                        return JsonConvert.DeserializeObject<object[]>(File.ReadAllText(FixturePath(name + ".json")));
                    }

        public object[] GlobalPhoneTestCases
        {
            get { return _globalPhoneTestCases ?? (_globalPhoneTestCases = JsonFixture("global_phone_test_cases")); }
        }

    }
}
