using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer))]
    [TestFixture(typeof(NewtonsoftDeserializer))]
	public class DatabaseGeneratorTests<Deserializer> : TestFixtureBase where Deserializer:IDeserializer, new()
    {
        public DatabaseGeneratorTests()
            : base(ForData.None)
        {
        }

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_deserializer = new Deserializer ();
		}
        [SetUp]
        public void SetUp()
        {
            Context.DefaultTerritoryName = "US";
        }

        [Test]
        public void it_can_generate_data_without_causing_an_exception() 
        {
            var generator = DatabaseGenerator.Load(PhoneNumberMetadata);
            generator.RecordData();
        }
    }
}
