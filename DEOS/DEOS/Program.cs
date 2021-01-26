using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DEOS
{
    class Program
    {
        public static List<string> GetStrings(byte[] data)
        {
            var stringLines = new List<string>();

            if (data == null || data.Length == 0)
            {
                return stringLines;
            }

            
            
            using (var ms = new MemoryStream(data, false))
            {
                using (var streamReader = new StreamReader(ms, Encoding.GetEncoding(1252), false, 2048, false))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();

                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        var matches = Regex.Matches(line, @"[ -~\t]{8,}", RegexOptions.Compiled);

                        foreach (Match match in matches)
                        {
                            if (string.IsNullOrWhiteSpace(match.Value))
                            {
                                continue;
                            }
                            
                            stringLines.Add(match.Value);
                        }
                    }
                }
            }

            return stringLines;
        }
        
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Need a file");

                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            var fileBytes = File.ReadAllBytes(args[0]);

            var fileStrings = GetStrings(fileBytes);

            var parsedResponse = DEOSParser.Parse(fileStrings);

            foreach (var key in parsedResponse)
            {
                Console.WriteLine($"{key.Key}:");

                if (!key.Value.Any())
                {
                    Console.WriteLine("<No matches>");

                    continue;
                }
                
                foreach (var match in parsedResponse[key.Key])
                {
                    Console.WriteLine(match);
                }
            }
        }
    }
}
