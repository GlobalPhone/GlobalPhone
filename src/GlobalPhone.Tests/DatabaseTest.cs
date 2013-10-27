using NUnit.Framework;

namespace GlobalPhone.Tests
{
    [TestFixture]
    public class DatabaseTest : TestFixtureBase
    {

        [Test]
        public void initializing_database_manually()
        {
            var db = new Database(record_data);
            assert_equal(record_data.Length, db.regions.Length);
        }
        [Test]
        public void finding_region_by_country_code()
        {
            var region = db.region(1);
            Assert.That(region,Is.TypeOf<Region>());
            assert_equal("1", region.country_code);
        }
        [Test]
        public void nonexistent_region_returns_nil()
        {
            var region = db.region(999);
            assert_nil(region);
        }
        [Test]
        public void finding_territory_by_name()
        {
            var territory = db.territory("gb");
            Assert.That(territory,Is.TypeOf<Territory>());
            assert_equal("GB", territory.name);
            //assert_equal "GB", territory.name
            assert_equal(db.region(44), territory.region);
        }

        [Test]
        public void nonexistent_territory_returns_nil()
        {
            var territory = db.territory("nonexistent");
            assert_nil(territory);
        }

    }
}