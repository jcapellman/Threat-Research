using System;
using SieProcessHollower.lib;
using SieProcessHollower.lib.Containers;

namespace SieProcessHollower
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new ProcessHollowerContainer(args);

            if (container.InError)
            {
                return;
            }

            var result = ProcessHollower.HollowProcess(container);

            if (!result)
            {
                Console.WriteLine("Failed to hollow process");
            }
        }
    }
}