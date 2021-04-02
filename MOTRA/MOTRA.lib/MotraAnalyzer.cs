using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MOTRA.lib.Analyzers.Base;
using MOTRA.lib.Container;

namespace MOTRA.lib
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

        public AnalysisContainer Analyze(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "FileName argument was null");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"File {fileName} was not found");
            }

            using var stream = File.OpenRead(fileName);

            var container = new AnalysisContainer
            {
                Analysis = new Dictionary<string, List<string>>(),
                FileType = null,
                Scannable = false
            };

            foreach (var analyzer in _analyzers)
            {
                try
                {
                    container.Scannable = true;
                    container.FileType = analyzer.Name;
                    container.Analysis = analyzer.Analyze(stream);

                    break;
                }
                catch (Exception)
                {
                    // Assume it just wasn't the proper type
                }
            }

            return container;
        }
    }
}