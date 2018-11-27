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
            var u = $"{_baseUrl}/{id}";
            var document = new HtmlWeb().Load(u);

            var node = document
                .GetElementbyId("tabs-last-pos")
                .ChildNodes[1]
                .ChildNodes[1]
                .ChildNodes[1]
                .ChildNodes[7]
                .ChildNodes[3]
                .FirstChild
                .FirstChild
                .InnerHtml;

            var split = node.Split('/');

            var lat = split[0].Split('&')[0].Trim();
            var lon = split[1].Split('&')[0].Trim();

            return new Location
            {
                Rig = { Id = id },
                Date = DateTime.Now,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lon)
            };
        }

        public IEnumerable<Location> GetMultipleLocations(IEnumerable<int> ids)
        {
            return ids.Select(GetLatestLocation).ToList();
        }
    }
}