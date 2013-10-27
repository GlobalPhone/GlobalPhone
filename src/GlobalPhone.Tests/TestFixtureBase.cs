using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace GlobalPhone.Tests
{
    public class TestFixtureBase
    {
        public void assert(bool assert)
        {
            if (!assert) throw new Exception();
        }

        public void assert_equal<T>(T expected, T actual)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        public void assert_nil<T>(T actual)
        {
            Assert.That(actual, Is.Null);
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
        private TestContext _context;
        public TestContext context
        {
            get { return _context ?? (_context = new TestContext {db_path = fixture_path("record_data.json")}); }
        }
        private string fixture_path(string file)
        {
            return Path.Combine("fixtures", file);//File.expand_path("../fixtures/#{filename}", __FILE__)
        }

        public Database db
        {
            get { return context.db; }
        }

        private object[] _record_data;
        private object[] _example_numbers;

        public object[] record_data
        {
            get { return _record_data ?? (_record_data = json_fixture("record_data")); }
        }
        public object[] example_numbers
        {
            get { return _example_numbers ?? (_example_numbers = json_fixture("example_numbers")); }
        }
        private object[] json_fixture(string name)
        {
            return new Makrill.JsonConvert().Deserialize(JArray.Parse(File.ReadAllText(fixture_path(name + ".json"))));
        }
    

    }
}
