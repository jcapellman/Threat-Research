using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MOTRA.lib;

namespace MOTRA.tests
{
    [TestClass]
    public class MotraAnalyzerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MotraAnalyzer_NullArgument()
        {
            var motra = new MotraAnalyzer();

            motra.Analyze(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MotraAnalyzer_InvalidFileArgument()
        {
            var motra = new MotraAnalyzer();

            motra.Analyze("wick");
        }
    }
}