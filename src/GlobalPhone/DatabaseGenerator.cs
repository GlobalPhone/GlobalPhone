using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GlobalPhone
{
    public class DatabaseGenerator
    {
        private Nokogiri.XmlDoc Doc { get; set; }

        private DatabaseGenerator(Nokogiri.XmlDoc doc)
        {
            Doc = doc;
        }

        public static DatabaseGenerator Load(string text)
        {
            return new DatabaseGenerator(Nokogiri.Xml(text));
        }
        public static DatabaseGenerator LoadFile(string filename)
        {
            return Load(File.ReadAllText(filename));
        }

        private IDictionary[] record_data_hash;
        public IDictionary[] RecordData()
        {
            return record_data_hash ?? (record_data_hash = TerritoryNodesByRegion().Map(kv =>
            {
                var countryCode = kv.Key;
                var territoryNodes = kv.ToArray();
                return
                    Truncate(CompileRegion(
                        territoryNodes,
                        countryCode));
            }).ToArray());
        }

        private string[][] _testCases;
        public string[][] TestCases()
        {
            return _testCases ?? (_testCases = TerritoryNodes().Map(ExampleNumbersForTerritoryNode)
                .Flatten(1).Cast<string[]>().Where(arr=>arr.Length>0).ToArray());
        }

        private IEnumerable<Nokogiri.Node> TerritoryNodes()
        {
            return Doc.Search("territory");
        }

        private static string TerritoryName(Nokogiri.Node node)
        {
            return node["id"];
        }

        private IEnumerable<string[]> ExampleNumbersForTerritoryNode(Nokogiri.Node node)
        {
            var name = TerritoryName(node);
            if (name == "001") return new[] { new string[0] };
            return node.Search(example_numbers_selector())
                .Map(node1 => new[] { node1.Text, name })
                .ToArray();
        }

        private IEnumerable<IGrouping<string, Nokogiri.Node>> TerritoryNodesByRegion()
        {
            return TerritoryNodes().GroupBy(node => node["countryCode"]);
        }

        private string example_numbers_selector()
        {
            return "./*[not(" + String.Join(" or ", ExampleNumberTypesToExclude().Map(type =>
                                                                                           "self::" + type)) +
                   ")]/exampleNumber";
        }

        private string[] ExampleNumberTypesToExclude()
        {
            return "emergency shortCode".Split(new[] { ' ' });
        }

        private IDictionary CompileRegion(IEnumerable<Nokogiri.Node> territoryNodes, string countryCode)
        {
            var nodes = territoryNodes.ToArray();
            var kv = CompileTerritories(nodes);
            var territories = kv.Item1;
            var mainTerritoryNode = kv.Item2;
            var formats = CompileFormats(nodes);

            return new Dictionary<string, object>
                     {
                         {"countryCode",countryCode},
                         {"formats",formats},
                         {"territories",territories},
                         {"interPrefix", mainTerritoryNode["internationalPrefix"]},
                         {"prefix",mainTerritoryNode["nationalPrefix"]},
                         {"prefixParse",Squish(mainTerritoryNode["nationalPrefixForParsing"])},
                         {"prefixTRule",Squish(mainTerritoryNode["nationalPrefixTransformRule"])}
                     };
        }

        private Tuple<object[], Nokogiri.Node> CompileTerritories(IEnumerable<Nokogiri.Node> territoryNodes)
        {
            var territories = new List<object>();
            var nodes = territoryNodes.ToArray();
            var mainTerritoryNode = nodes.First();
            foreach (var node in nodes)
            {
                var territory = Truncate(CompileTerritory(node));
                if (node["mainCountryForCode"] != null)
                {
                    mainTerritoryNode = node;
                    territories.Insert(0, territory);
                }
                else
                {
                    territories.Add(territory);
                }
            }

            return new Tuple<object[], Nokogiri.Node>(territories.ToArray(), mainTerritoryNode);
        }

        private IDictionary CompileTerritory(Nokogiri.Node node)
        {
            return new Dictionary<string, object>
            {
                {"name",TerritoryName(node)},
                {"possibleNumber",Pattern(node, "generalDesc possibleNumberPattern")},
                {"nationalNumber",Pattern(node, "generalDesc nationalNumberPattern")},
                {"formattingRule",Squish(node["nationalPrefixFormattingRule"])}
            };
        }

        private IEnumerable<IDictionary> CompileFormats(IEnumerable<Nokogiri.Node> territoryNodes)
        {
            return Truncate(FormatNodesFor(territoryNodes).Map(node => Truncate(CompileFormat(node))));
        }

        private IDictionary CompileFormat(Nokogiri.Node node)
        {
            var format = new Dictionary<string, object>
                                    {
                                        {"pattern",node["pattern"]}, 
                                        {"format",TextOrNull(node, "format")}, 
                                        {"leadingDigits",Pattern(node, "leadingDigits")}, 
                                        {"formatRule",node["nationalPrefixFormattingRule"]}, 
                                        {"intlFormat",TextOrNull(node, "intlFormat")}, 
                                    };
            return format;
        }

        private IEnumerable<Nokogiri.Node> FormatNodesFor(IEnumerable<Nokogiri.Node> territoryNodes)
        {
            return territoryNodes.Map(node =>
                                       node.Search("availableFormats numberFormat").ToArray()).Flatten<Nokogiri.Node>();
        }
        private static string Squish(string @string)
        {
            return !String.IsNullOrEmpty(@string) ? @string.Gsub(@"\s+", "") : @string;
        }

        private string Pattern(Nokogiri.Node node, string selector)
        {
            return Squish(TextOrNull(node, selector));
        }
        private static string TextOrNull(Nokogiri.Node node, string selector)
        {
            var nodes = node.Search(selector);
            return nodes.IsEmpty() ? null : String.Join("", nodes.Map(n => n.Text));

        }

        private static IDictionary Truncate(IDictionary self)
        {
            var truncated = new Dictionary<string, object>();
            foreach (string key in self.Keys)
            {
                var value = self[key];
                if (value != null)
                {
                    truncated.Add(key,value);
                }
            }
            return truncated;
        }

        private static T[] Truncate<T>(IEnumerable<T> self)
        {
            return Truncate(self.ToArray());
        }

        private static T[] Truncate<T>(T[] self)
        {
            /*     def truncate(array)
array.dup.tap do |result|
result.pop while result.any? && result.last.nil?
end
end
*/
            var found = -1;
            for (int i = self.Length - 1; i >= 0; i--)
            {
                if (self[i] != null)
                {
                    found = i;
                    break;
                }
            }
            if (found >= 0 && found != self.Length - 1)
            {
                return self.Take(found + 1).ToArray();
            }
            return self.ToArray();
        }

    }
}
