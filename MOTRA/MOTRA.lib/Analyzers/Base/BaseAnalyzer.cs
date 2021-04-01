using System.Collections.Generic;
using System.IO;

using DocumentFormat.OpenXml.Packaging;

namespace MOTRA.lib.Analyzers.Base
{
    public abstract class BaseAnalyzer
    {
        protected abstract List<string> GetUrls(OpenXmlPackage package);

        public abstract Dictionary<string, List<string>> Analyze(Stream fileStream);
    }
}