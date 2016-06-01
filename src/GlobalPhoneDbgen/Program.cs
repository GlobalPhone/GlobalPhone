using System;
using System.Text.RegularExpressions;
using GlobalPhone;
using System.IO;
#if NEWTONSOFT
using Newtonsoft.Json;
#else
using System.Web.Script.Serialization;
#endif
namespace GlobalPhoneDbgen
{
    class Program
    {
        private const string RemoteUrl = "https://raw.githubusercontent.com/googlei18n/libphonenumber/master/resources/PhoneNumberMetadata.xml";

        static void Usage(string nameOfProgram)
        {
            Warn("Usage: " + nameOfProgram + @" [--compact] [--test] [<filename> | <url>]");
        }

        private static void Warn(string str)
        {
            Console.WriteLine(str);
        }

        static void Help()
        {
#if NEWTONSOFT
            var showCompat = true;
#else
            var showCompat = false;
#endif
            Warn(@" Generates a database for the Ruby GlobalNumber library in JSON format
    and writes it to standard output.

    Specify either a local path or URL pointing to a copy of Google's
    libphonenumber PhoneNumberMetaData.xml file.

    Omit the filename argument to download and use the latest version of
    Google's database from:
      " + RemoteUrl + @"

Options:" + (showCompat ? @"
    --compact      Strip all whitespace from the JSON output" : "") + @"
    --test         Generate example phone number fixtures for smoke tests

");
        }

        static void Main(string[] args)
        {
            const string nameOfProgram = "GlobalPhoneDbgen.exe";
            var path = RemoteUrl;
            var method = "record_data";
#if NEWTONSOFT
            var compact = false;
#endif
            foreach (var arg in args)
            {

                switch (arg)
                {
#if NEWTONSOFT
                    case "-c":
                    case "--compact":
                        compact = true;
                        break;
#endif
                    case "-t":
                    case "--test":
                        method = "test_cases";
                        break;
                    case "-h":
                    case "--help":
                        Usage(nameOfProgram);
                        Help();
                        Environment.Exit(1);
                        break;
                    default:
                        if (Regex.IsMatch(arg, "^-"))
                        {
                            Warn(nameOfProgram + ": unknown option `" + arg + @"'");
                        }
                        else
                        {
                            path = arg;
                        }
                        break;
                }
            }
            string dl = File.Exists(path) 
                ? File.ReadAllText(path) 
                : new System.Net.WebClient().DownloadString(path);
            var generator = DatabaseGenerator.Load(dl);
            var result = Send(generator, method);
            Console.WriteLine(
#if NEWTONSOFT
wJsonConvert.SerializeObject(result, compact ? Formatting.None : Formatting.Indented)
#else
new JavaScriptSerializer().Serialize(result)
#endif
);

        }

        private static object Send(DatabaseGenerator generator, string method)
        {
            switch (method)
            {
                case "record_data":
                    return generator.RecordData();
                case "test_cases":
                    return generator.TestCases();
                default:
                    throw new Exception("Unknown method " + method);
            }
        }
    }
}
