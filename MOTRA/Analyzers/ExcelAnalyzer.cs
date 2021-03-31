using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using MOTRA.Analyzers.Base;

namespace MOTRA.Analyzers
{
    public class ExcelAnalyzer : BaseAnalyzer
    {
        private Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

        private static List<string> ParsePart<T>(SpreadsheetDocument document) where T : OpenXmlPart
        {
            var parts = new List<T>();
        
            foreach (var worksheet in document.WorkbookPart.WorksheetParts)
            {
                var sheetParts = worksheet?.GetPartsOfType<T>().ToList();

                if (sheetParts != null)
                {
                    parts.AddRange(sheetParts);
                }
            }
            
            return parts.Select(a => a.Uri.ToString()).ToList();
        }

        protected static List<string> GetURLsFromString(string str)
        {
            var urls = new List<string>();

            var pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase);

            var matches = regEx.Matches(str);

            foreach (Match match in matches)
            {
                urls.Add(match.Value);
            }

            return urls;
        }

        protected override List<string> GetUrls(OpenXmlPackage package)
        {
            var spreadsheet = (SpreadsheetDocument) package;

            var urls = new List<string>();

            foreach (var worksheet in spreadsheet.WorkbookPart.WorksheetParts)
            {
                var sheet = worksheet.Worksheet;

                var rows = sheet.GetFirstChild<SheetData>().Elements<Row>();

                foreach (var cell in rows.SelectMany(row => row.Elements<Cell>().ToList()))
                {
                    if (cell.DataType == null)
                    {
                        continue;
                    }

                    var cellValue = cell.InnerText;

                    switch (cell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            var stringTable = spreadsheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>()
                                .FirstOrDefault();

                            if (stringTable == null)
                            {
                                continue;
                            }

                            var cellText = stringTable.SharedStringTable.ElementAt(int.Parse(cellValue)).InnerText;

                            if (cellText.Length > 2)
                            {
                                urls.AddRange(GetURLsFromString(cellText));
                            }

                            break;
                    }
                }
            }

            return urls;
        }

        private void AddResult<T>(SpreadsheetDocument document, string name) where T: OpenXmlPart
        {
            var result = ParsePart<T>(document);
            
            results.Add(name, result);
        }

        public override Dictionary<string, List<string>> Analyze(Stream fileStream)
        {
            try
            {
                using var spreadsheet = SpreadsheetDocument.Open(fileStream, false);

                AddResult<MacroSheetPart>(spreadsheet, "Macros");
                AddResult<ConnectionsPart>(spreadsheet, "External Connections");
                AddResult<VbaProjectPart>(spreadsheet, "VBA Project");
                AddResult<EmbeddedObjectPart>(spreadsheet, "Embedded Objects");
                AddResult<ImagePart>(spreadsheet, "Images");
                AddResult<CustomDataPart>(spreadsheet, "Custom Data");
                AddResult<VbaDataPart>(spreadsheet, "VBA Data");
                AddResult<VmlDrawingPart>(spreadsheet, "VML Drawing");
                
                var urls = GetUrls(spreadsheet);

                results.Add("Cell URLS", urls);
            }
            catch (OpenXmlPackageException)
            {
                results.Add("Error", new List<string> { "File is not a Modern Excel Document"});
            }

            return results;
        }
    }
}