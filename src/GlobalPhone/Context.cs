namespace GlobalPhone
{
    public class Context
    {
        private Database _db;
        public string db_path { get; set; }

        public virtual Database db
        {
            get { return _db ?? (_db = Database.load_file(db_path.Unless(new NoDatabaseException("set `db_path=' first")))); }
        }

        public Context()
        {
            default_territory_name = "US";
        }

        public string default_territory_name { get; set; }
        public Number parse(string str, string territory_name = null)
        {
            return db.parse(str, territory_name ?? default_territory_name);
        }
        public string normalize(string str, string territory_name = null)
        {
            var number = db.parse(str, territory_name ?? default_territory_name);
            return number!=null?number.international_string:null;
        }

        public bool validate(string str, string territory_name = null)
        {
            var number = db.parse(str, territory_name ?? default_territory_name);
            return number.NotNull() && number.valid();
        }

    }
}