using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class DatabaseTest : TestFixtureBase
    {

        [Test]
        public void initializing_database_manually()
        {
            var db = new Database(RecordData);
            assert_equal(RecordData.Length, db.Regions.Length);
        }
        [Test]
        public void finding_region_by_country_code()
        {
            var region = Db.Region(1);
            NUnit.Framework.Assert.That(region,Is.TypeOf<Region>());
            assert_equal("1", region.CountryCode);
        }
        [Test]
        public void nonexistent_region_returns_nil()
        {
            var region = Db.Region(999);
            assert_nil(region);
        }
        [Test]
        public void finding_territory_by_name()
        {
            var territory = Db.Territory("gb");
            NUnit.Framework.Assert.That(territory,Is.TypeOf<Territory>());
            assert_equal("GB", territory.Name);
            //assert_equal "GB", territory.name
            assert_equal(Db.Region(44), territory.Region);
        }

        [Test]
        public void nonexistent_territory_returns_nil()
        {
            var territory = Db.Territory("nonexistent");
            assert_nil(territory);
        }

    }
}