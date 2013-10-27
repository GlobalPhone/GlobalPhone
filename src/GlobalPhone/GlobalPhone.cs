namespace GlobalPhone
{
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
    }
}
