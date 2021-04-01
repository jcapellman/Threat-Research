using System.Collections.Generic;

namespace MOTRA.lib.Container
{
    public class AnalysisContainer
    {
        public bool Scannable { get; set; }

        public string FileType { get; set; }

        public Dictionary<string, List<string>> Analysis { get; set; }
    }
}