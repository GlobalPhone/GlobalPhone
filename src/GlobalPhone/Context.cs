using System;

namespace GlobalPhone
{
    public class Context
    {
        private Database _db;
        public string DbPath { get; set; }
        public string DbText { get; set; }
        public virtual Database Db
        {
            get
            {
                return _db ?? (_db = !string.IsNullOrEmpty(DbText)
                                                        ? Database.Load(DbText)
                                                        : Database.LoadFile(DbPath.Unless(new NoDatabaseException("set `DbPath=' first"))));
            }
        }

        public Context()
        {
            DefaultTerritoryName = "US";
        }

        public string DefaultTerritoryName { get; set; }
        public Number Parse(string str, string territoryName = null)
        {
            return Db.Parse(str, territoryName ?? DefaultTerritoryName);
        }
        public string Normalize(string str, string territoryName = null)
        {
            var number = Db.Parse(str, territoryName ?? DefaultTerritoryName);
            return number != null ? number.InternationalString : null;
        }

        public bool Validate(string str, string territoryName = null)
        {
            try
            {
                var number = Db.Parse(str, territoryName ?? DefaultTerritoryName);
                return number.NotNull() && number.IsValid;
            }
            catch (FailedToParseNumberException)
            {
                return false;
            }
        }

    }
}