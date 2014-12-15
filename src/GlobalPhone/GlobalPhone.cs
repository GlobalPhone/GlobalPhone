namespace GlobalPhone
{
    /// <summary>
    /// Static class that holds a single Context
    /// </summary>
    public class GlobalPhone
    {
        private static Context _context;

        private static Context Context
        {
            get { return _context ?? (_context = new Context()); }
        }

        public static bool Validate(string number, string territoryName = null)
        {
            return Context.Validate(number, territoryName);
        }
        public static Number Parse(string number, string territoryName = null)
        {
            return Context.Parse(number, territoryName);
        }
        public static string Normalize(string number, string territoryName = null)
        {
            return Context.Normalize(number, territoryName);
        }

        public static bool TryParse(string str, out Number number, string territoryName = null)
        {
            return Context.TryParse(str, out number, territoryName);
        }

        public static bool TryNormalize(string str, out string number, string territoryName = null)
        {
            return Context.TryNormalize(str, out number, territoryName);
        }

        public static string DbPath
        {
            get { return Context.DbPath; }
            set { Context.DbPath = value; }
        }

        public static string DbText
        {
            get { return Context.DbText; }
            set { Context.DbText = value; }
        }

        public string DefaultTerritoryName
        {
            get { return Context.DefaultTerritoryName; }
            set { Context.DefaultTerritoryName = value; }
        }
    }
}
