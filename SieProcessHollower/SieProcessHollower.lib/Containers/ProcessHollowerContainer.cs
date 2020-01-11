using System;
using System.Diagnostics;
using System.IO;

using SieProcessHollower.lib.Helpers;

namespace SieProcessHollower.lib.Containers
{
    public class ProcessHollowerContainer
    {
        public Process TargetProcess { get; private set; }

        public string TargetProcessName { get; private set; }

        public byte[] SourceBytes { get; private set; }

        public string SourceFileName { get; private set; }

        public byte[] ShellCodeBytes { get; private set; }

        public string ShellCodeFileName { get; private set; }

        public bool InError { get; private set; }

        public ProcessHollowerContainer(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Requires three parameters: <source target> <target process name> <shellcode>");
                Console.WriteLine("Exiting");

                InError = true;

                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"{args[0]} does not exist - exiting");

                InError = true;

                return;
            }
            else
            {
                SourceBytes = File.ReadAllBytes(args[0]);
                SourceFileName = args[0];
            }

            TargetProcess = ProcessHelper.GetProcessFromPath(args[1]);

            if (TargetProcess == null)
            {
                Console.WriteLine($"Target process {args[1]} was not found - exiting");

                InError = true;

                return;
            }
            else
            {
                TargetProcessName = args[1];
            }

            if (!File.Exists(args[2]))
            {
                Console.WriteLine($"Shellcode {args[2]} was not found - exiting");

                InError = true;

                return;
            }
            else
            {
                ShellCodeBytes = File.ReadAllBytes(args[2]);
                ShellCodeFileName = args[2];
            }
        }
    }
}