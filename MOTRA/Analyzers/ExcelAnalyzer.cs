using System.Collections.Generic;
using System.IO;
using System.Linq;

using DocumentFormat.OpenXml.Packaging;

using MOTRA.Analyzers.Base;

namespace MOTRA.Analyzers
{
    public class ExcelAnalyzer : BaseAnalyzer
    {
        private static string ParsePart<T>(SpreadsheetDocument document, string name) where T : OpenXmlPart
        {
            var parts = document.WorkbookPart.WorksheetParts.Sum(a => a.GetPartsOfType<T>().Count());

            // TODO: Output elements instead of just the count

            return parts > 0
                ? $"{name} - {parts} were found"
                : $"{name} - None were found";
        }
        
        public override List<string> Analyze(Stream fileStream)
        {
            var analysis = new List<string>();
            
            try
            {
                using var spreadsheet = SpreadsheetDocument.Open(fileStream, false);

                analysis.Add(ParsePart<MacroSheetPart>(spreadsheet, "Macros"));
                analysis.Add(ParsePart<ConnectionsPart>(spreadsheet, "External Connections"));
                analysis.Add(ParsePart<VbaProjectPart>(spreadsheet, "VBA Project"));
                analysis.Add(ParsePart<EmbeddedObjectPart>(spreadsheet, "Embedded Objects"));
                analysis.Add(ParsePart<ImagePart>(spreadsheet, "Images"));
                analysis.Add(ParsePart<CustomDataPart>(spreadsheet, "Custom Data"));
                analysis.Add(ParsePart<VbaDataPart>(spreadsheet, "VBA Data"));
                analysis.Add(ParsePart<VmlDrawingPart>(spreadsheet, "VML Drawing"));
            }
            catch (OpenXmlPackageException)
            {
                analysis.Add("File is not a Modern Excel Document");
            }

            return analysis;
        }
    }
}