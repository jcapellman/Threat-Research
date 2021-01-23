using System;
using System.IO;

namespace EEM
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file specified to embed");

                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Could not find {args[0]} file");

                return;
            }

            var embedBytes = File.ReadAllBytes(args[0]);
            
            // TODO: Encrypt bytes
            // TODO: Run template
            // TODO: Compile binary
        }
    }
}