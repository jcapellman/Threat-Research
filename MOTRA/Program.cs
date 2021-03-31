using System;
using System.IO;
using System.Linq;

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

            var analysis = new MotraAnalyzer().Analyze(args[0]);

            foreach (var key in analysis.Keys)
            {
                Console.WriteLine($"{key} - {(analysis[key].Any() ? string.Join(", ", analysis[key]) : "none found")}");
            }
        }
    }
}