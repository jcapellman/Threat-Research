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
        private static string ParsePart<T>(SpreadsheetDocument document, string name) where T : OpenXmlPart
        {
            var parts = document.WorkbookPart.WorksheetParts.Sum(a => a.GetPartsOfType<T>().Count());

            // TODO: Output elements instead of just the count

            return parts > 0
                ? $"{name} - {parts} were found"
                : $"{name} - None were found";
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

                var urls = GetUrls(spreadsheet);

                analysis.Add(urls.Any() ? $"URLS - {urls.Count} were found ({string.Join(", ", urls)})" : "URLS - None were found");
            }
            catch (OpenXmlPackageException)
            {
                analysis.Add("File is not a Modern Excel Document");
            }

            return analysis;
        }
    }
}