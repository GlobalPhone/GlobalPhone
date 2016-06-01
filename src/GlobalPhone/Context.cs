using System.Web.Script.Serialization;

namespace GlobalPhone
{
    public class DefaultDeserializer : IDeserializer
    {
        JavaScriptSerializer implementation = new JavaScriptSerializer();
        public DefaultDeserializer()
        {
        }

        public object[] Deserialize(string text)
        {
            return implementation.Deserialize<object[]>(text);
        }
    }

    public interface IDeserializer
    {
        object[] Deserialize(string text);
    }

    public class Context
    {
        public Context(IDeserializer serializer = null)
        {
            DefaultTerritoryName = "US";
            _serializer = serializer ?? new DefaultDeserializer();
        }
        private Database _db;
        private readonly IDeserializer _serializer;

        public string DbPath { get; set; }
        public string DbText { get; set; }
        public virtual Database Db
        {
            get
            {
                return _db ?? (_db = !string.IsNullOrEmpty(DbText)
                    ? Database.Load(DbText, _serializer)
                    : Database.LoadFile(DbPath.ThrowIfNullOrEmpty(new NoDatabaseException("set `DbPath=' first")), _serializer));
            }
        }

        public string DefaultTerritoryName { get; set; }
        public Number Parse(string str, string territoryName = null)
        {
            return Db.Parse(str, territoryName ?? DefaultTerritoryName);
        }

        public bool TryParse(string str, out Number number, string territoryName = null)
        {
            try
            {
                number = Parse(str, territoryName);
                return true;
            }
            catch (FailedToParseNumberException)
            {
            }
            catch (UnknownTerritoryException)
            {
            }
            number = null;
            return false;
        }

        public string Normalize(string str, string territoryName = null)
        {
            var number = Db.Parse(str, territoryName ?? DefaultTerritoryName);
            return number != null ? number.InternationalString : null;
        }

        public bool TryNormalize(string str, out string number, string territoryName = null)
        {
            try
            {
                number = Normalize(str, territoryName);
                return true;
            }
            catch (FailedToParseNumberException)
            {
            }
            catch (UnknownTerritoryException)
            {
            }
            number = null;
            return false;
        }

        public bool Validate(string str, string territoryName = null)
        {
            try
            {
                var number = Db.Parse(str, territoryName ?? DefaultTerritoryName);
                return number != null && number.IsValid;
            }
            catch (FailedToParseNumberException)
            {
                return false;
            }
            catch (UnknownTerritoryException)
            {
                return false;
            }
        }

    }
}