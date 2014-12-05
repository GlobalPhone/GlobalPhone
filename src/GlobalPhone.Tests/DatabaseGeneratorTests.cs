using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHash)]
    [TestFixture(typeof(DefaultDeserializer), ForData.UseArray)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseArray)]
	public class DatabaseGeneratorTests<Deserializer> : TestFixtureBase where Deserializer:IDeserializer, new()
    {
        public DatabaseGeneratorTests(ForData forData)
            : base(forData)
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
