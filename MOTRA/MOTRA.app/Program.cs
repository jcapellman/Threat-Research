using System;
using System.IO;
using System.Linq;

using MOTRA.lib;

namespace MOTRA
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"{args[0]} does not exist - exiting");

                return;
            }

            Console.WriteLine($"Analysis of {args[0]}:");

            var result = new MotraAnalyzer().Analyze(args[0]);

            if (!result.Scannable)
            {
                Console.WriteLine("File was not scannable");

                return;
            }

            Console.WriteLine($"File classified as {result.FileType}");

            foreach (var key in result.Analysis.Keys)
            {
                Console.WriteLine($"{key} - {(result.Analysis[key].Any() ? string.Join(", ", result.Analysis[key]) : "none found")}");
            }
        }
    }
}