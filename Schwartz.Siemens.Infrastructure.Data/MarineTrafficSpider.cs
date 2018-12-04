using HtmlAgilityPack;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data
{
    public class MarineTrafficSpider : IWebSpider
    {
        private readonly string _baseUrl;

        public MarineTrafficSpider(string url)
        {
            _baseUrl = url;
        }

        public Location GetLatestLocation(int id)
        {
            var u = $"{_baseUrl}{id}";
            var document = new HtmlWeb().Load(u);

            var posDateString = LastPositionTabs(document, 1); // Position Received. Can be parsed to datetime
            var area = LastPositionTabs(document, 5); // Area
            var posString = LastPositionTabs(document, 7); // Location. Lat and Long are together, Break it up
            var status = LastPositionTabs(document, 9); // Status

            var date = ProcessDate(posDateString);
            ProcessPosition(posString, out var lat, out var lon);

            return new Location
            {
                Rig = new Rig { Imo = id },
                Date = date,
                Latitude = lat,
                Longitude = lon,
                Area = area,
                Status = status,
            };
        }

        public IEnumerable<Location> GetMultipleLocations(IEnumerable<int> ids)
        {
            return ids.Select(GetLatestLocation).ToList();
        }

        private string LastPositionTabs(HtmlDocument doc, int node)
        {
            var raw = doc.GetElementbyId("tabs-last-pos")
                .ChildNodes[1].ChildNodes[1].ChildNodes[1]
                .ChildNodes[node]
                .ChildNodes[3]
                .FirstChild
                .InnerHtml;

            return Sanitize(raw);
        }

        private DateTime ProcessDate(string dateString)
        {
            return DateTime.Parse(dateString);
        }

        private void ProcessPosition(string source, out double lat, out double lon)
        {
            var split = source
                .Replace('.', ',')
                .Split('/');

            lat = double.Parse(split[0].Split('&')[0].Trim());
            lon = double.Parse(split[1].Split('&')[0].Trim());
        }

        private string Sanitize(string raw)
        {
            while (true)
            {
                // Removes HTML tags
                if (raw.Contains('<'))
                {
                    var open = raw.IndexOf('<');
                    var end = raw.IndexOf('>');

                    var beginning = raw.Substring(0, open);
                    var substring = raw.Substring(end + 1);

                    raw = beginning + substring;
                    continue;
                }

                // Removes double spacing inside string
                if (raw.Contains("  "))
                {
                    var replace = raw.Replace("  ", " ");
                    raw = replace;
                    continue;
                }

                /*
                 * Meant for the time stamps.
                 * When the extracted text is formatted like "updated 5 minutes ago ([date] (UTC))
                 * this section will extract [date] only, cutting off the surrounding text
                 */
                if (raw.Contains('('))
                {
                    var open = raw.IndexOf('(');
                    var mid = raw.Substring(open + 1);
                    var close = mid.IndexOf('(');
                    raw = mid.Substring(0, close);
                    continue;
                }

                /*
                 * This section is meant for text formatted like "[date] UTC"
                 * If the extracted date contains UTC it will be removed
                 */
                if (raw.ToLower().Contains(" utc"))
                {
                    raw = raw
                        .ToLower()
                        .Replace(" utc", "");
                }

                return raw;
            }
        }
    }
}