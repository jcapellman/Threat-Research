using System;

using SieProcessHollower.lib.Containers;
using SieProcessHollower.lib.Helpers;

namespace SieProcessHollower.lib
{
    public class ProcessHollower
    {
        public static bool HollowProcess(ProcessHollowerContainer container)
        {
            var sourceProcessInfo = ProcessHelper.StartSuspendedProcess(container.SourceFileName);

            Console.WriteLine($"Suspended {container.SourceFileName} successfully...");

            // TODO: Remap shell code into target process
            // TODO: Read map on the fly

            ProcessHelper.CloseProcessHandles(sourceProcessInfo);

            Console.WriteLine($"Freed thread and handle on target process...");

            return true;
        }
    }
}