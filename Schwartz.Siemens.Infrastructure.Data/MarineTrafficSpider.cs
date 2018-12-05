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

        public Location GetLatestLocation(int imo)
        {
            var u = $"{_baseUrl}{imo}";
            var document = new HtmlWeb().Load(u);

            GetPosition(document,
                out var date, out var area, out var lat, out var lon, out var status);

            return new Location
            {
                Rig = new Rig { Imo = imo },
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

        public Rig GetRig(int imo)
        {
            var u = $"{_baseUrl}{imo}";
            var document = new HtmlWeb().Load(u);

            var rigName = GetRigName(document);

            return new Rig
            {
                Imo = imo,
                Name = rigName,
                Outdated = false,
                Locations = new List<Location>
                {
                    GetLatestLocation(imo),
                },
            };
        }

        private void GetPosition(
            HtmlDocument document, out DateTime date, out string area,
            out double lat, out double lon, out string status)
        {
            date = ProcessDate(LastPositionTabs(document, 1));
            area = LastPositionTabs(document, 5);
            ProcessPosition(LastPositionTabs(document, 7), out lat, out lon);
            status = LastPositionTabs(document, 9);
        }

        private string GetRigName(HtmlDocument document)
        {
            var raw = document.DocumentNode
                .SelectSingleNode("//body")
                .SelectSingleNode("main")
                .ChildNodes[1].ChildNodes[1]
                .ChildNodes[1].ChildNodes[9]
                .ChildNodes[1].ChildNodes[1]
                .ChildNodes[1].ChildNodes[1]
                .ChildNodes[3]
                .InnerHtml;

            return Sanitize(raw);
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