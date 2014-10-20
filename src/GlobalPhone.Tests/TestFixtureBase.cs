using System.IO;
using NUnit.Framework;

namespace GlobalPhone.Tests
{
    public class TestFixtureBase
    {
		[TearDown]
        public void TearDown()
        {
            _context = null;
        }

        private Context _context;
        public Context Context
        {
			get { return _context ?? (_context = new Context(_deserializer) { DbPath = FixturePath("record_data.json") }); }
        }
        private static string FixturePath(string file)
        {
            return Path.Combine("fixtures", file);
        }

        public Database Db
        {
            get { return Context.Db; }
        }

        private object[] _recordData;
        private object[] _exampleNumbers;
        private object[] _globalPhone;
        private object[] _globalPhone2;
        private object[] _globalPhoneTestCases;
        private string _phoneNumberMetadata;
        private string _phoneNumberMetadata2;
        private object[] _exampleInvalidNumbers;

        public object[] RecordData
        {
            get { return _recordData ?? (_recordData = JsonFixture("record_data")); }
        }
        public object[] ExampleNumbers
        {
            get { return _exampleNumbers ?? (_exampleNumbers = JsonFixture("example_numbers")); }
        }
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
		protected IDeserializer _deserializer;

        private object[] JsonFixture(string name)
        {
			return _deserializer.Deserialize(File.ReadAllText(FixturePath(name + ".json")));
        }

        public object[] GlobalPhone
        {
            get { return _globalPhone ?? (_globalPhone = JsonFixture("global_phone")); }
        }
        public object[] GlobalPhone2
        {
            get { return _globalPhone2 ?? (_globalPhone2 = JsonFixture("global_phone2")); }
        }
        public object[] GlobalPhoneTestCases
        {
            get { return _globalPhoneTestCases ?? (_globalPhoneTestCases = JsonFixture("global_phone_test_cases")); }
        }

        public string PhoneNumberMetadata
        {
            get { return _phoneNumberMetadata ?? (_phoneNumberMetadata = File.ReadAllText(FixturePath("PhoneNumberMetadata.xml"))); }
        }
        public string PhoneNumberMetadata2
        {
            get { return _phoneNumberMetadata2 ?? (_phoneNumberMetadata2 = File.ReadAllText(FixturePath("PhoneNumberMetadata2.xml"))); }
        }   

    }
}
