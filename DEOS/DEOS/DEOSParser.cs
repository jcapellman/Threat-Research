using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DEOS
{
    public class DEOSParser
    {
        public static Dictionary<string, List<string>> Parse(List<string> lines)
        {
            var response = new Dictionary<string, List<string>> { { "URL", new List<string>() } };

            var urlMatches = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                try
                {
                    var baseBytes = System.Convert.FromBase64String(line);

                    var unEncoded = System.Text.Encoding.UTF8.GetString(baseBytes);

                    foreach (Match match in urlMatches.Matches(unEncoded))
                    {
                        response["URL"].Add(match.Value);
                    }

                    foreach (Match match in urlMatches.Matches(line))
                    {
                        response["URL"].Add(match.Value);
                    }
                }
                catch (Exception)
                {

                }
            }

            return response;
        }
    }
}