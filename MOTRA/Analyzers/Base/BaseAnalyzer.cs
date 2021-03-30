using System.Collections.Generic;
using System.IO;

using DocumentFormat.OpenXml.Packaging;

namespace MOTRA.Analyzers.Base
{
    public abstract class BaseAnalyzer
    {
        protected abstract List<string> GetUrls(OpenXmlPackage package);

        public abstract List<string> Analyze(Stream fileStream);
    }
}