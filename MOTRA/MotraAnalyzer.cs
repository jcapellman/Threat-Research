using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MOTRA.Analyzers.Base;

namespace MOTRA
{
    public class MotraAnalyzer
    {
        private readonly List<BaseAnalyzer> _analyzers;

        public MotraAnalyzer()
        {
            _analyzers = typeof(MotraAnalyzer).Assembly.GetTypes()
                .Where(a => a.BaseType == typeof(BaseAnalyzer) && !a.IsAbstract)
                .Select(b => (BaseAnalyzer) Activator.CreateInstance(b)).ToList();
        }

        public Dictionary<string, List<string>> Analyze(string fileName)
        {
            using var stream = File.OpenRead(fileName);

            foreach (var analyzer in _analyzers)
            {
                try
                {
                    return analyzer.Analyze(stream);
                }
                catch (Exception)
                {
                    // Assume it just wasn't the proper type
                }
            }

            return null;
        }
    }
}