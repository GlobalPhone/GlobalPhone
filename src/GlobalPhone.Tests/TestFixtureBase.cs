using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace GlobalPhone.Tests
{
    public class TestFixtureBase
    {
        public void Assert(bool assert)
        {
            if (!assert) throw new Exception();
        }

        public void assert_equal<T>(T expected, T actual)
        {
            NUnit.Framework.Assert.That(actual, Is.EqualTo(expected));
        }

        public void assert_nil<T>(T actual)
        {
            NUnit.Framework.Assert.That(actual, Is.Null);
        }


        [SetUp]
        public void SetUp()
        {
        }
        [SetUp]
        public void TearDown()
        {
            _context = null;
        }
        private Context _context;
        public Context Context
        {
            get { return _context ?? (_context = new Context { DbPath = FixturePath("record_data.json") }); }
        }
        private static string FixturePath(string file)
        {
            return Path.Combine("fixtures", file);//File.expand_path("../fixtures/#{filename}", __FILE__)
        }

        public Database Db
        {
            get { return Context.Db; }
        }

        private object[] _recordData;
        private object[] _exampleNumbers;

        public object[] RecordData
        {
            get { return _recordData ?? (_recordData = json_fixture("record_data")); }
        }
        public object[] ExampleNumbers
        {
            get { return _exampleNumbers ?? (_exampleNumbers = json_fixture("example_numbers")); }
        }
        private object[] json_fixture(string name)
        {
            return new Makrill.JsonConvert().Deserialize(JArray.Parse(File.ReadAllText(FixturePath(name + ".json"))));
        }
    

    }
}
