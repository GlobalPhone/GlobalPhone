using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture(typeof(DefaultDeserializer), ForData.UseHash)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV2)]
    [TestFixture(typeof(NewtonsoftDeserializer), ForData.UseHashV3)]
    public class DatabaseTest<Deserializer> : TestFixtureBase where Deserializer : IDeserializer, new()
    {
        public DatabaseTest(ForData forData)
            : base(forData)
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            _deserializer = new Deserializer();
        }

        [Test]
        public void initializing_database_manually()
        {
            var db = new Database(RecordData);
            Assert.That(db.Regions.Length, Is.EqualTo(RecordData.Length));
        }
        [Test]
        public void finding_region_by_country_code()
        {
            Region region = Db.TryGetRegion(1);
            Assert.That(region, Is.TypeOf<Region>());
            Assert.That(region.CountryCode, Is.EqualTo("1"));
        }
        [Test]
        public void nonexistent_region_returns_nil()
        {
            var region = Db.TryGetRegion(999);
            Assert.That(region, Is.Null);
        }
        [Test]
        public void finding_territory_by_name()
        {
            var territory = Db.TryGetTerritory("gb");
            Assert.That(territory, Is.TypeOf<Territory>());
            Assert.That(territory.Name, Is.EqualTo("GB"));
            //assert_equal "GB", territory.name
            Assert.That(territory.Region, Is.EqualTo(Db.TryGetRegion(44)));
        }

        [Test]
        public void nonexistent_territory_returns_nil()
        {
            var territory = Db.TryGetTerritory("nonexistent");
            Assert.That(territory, Is.Null);
        }

    }
}