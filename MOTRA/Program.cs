using System;
using System.IO;

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

            var x = 1;

            foreach (var line in analysis)
            {
                Console.WriteLine($"{x}.{line}");

                x++;
            }
        }
    }
}