using System.Collections.Generic;
using System.IO;

namespace MOTRA.Analyzers.Base
{
    public abstract class BaseAnalyzer
    {
        public abstract List<string> Analyze(Stream fileStream);
    }
}