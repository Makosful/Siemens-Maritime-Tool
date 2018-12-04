using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.Entities;
using System.Collections.Generic;
using System.IO;

namespace Schwartz.Siemens.Infrastructure.FileReader
{
    public class CsvFileReader : IFileReader<Country>
    {
        public IEnumerable<Country> GetAllContent(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var countries = new List<Country>();

                reader.ReadLine(); // Ignores first line
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var split = line.Split(',');
                    for (var i = 0; i < split.Length; i++)
                    {
                        split[i] = split[i].Trim();
                    }

                    var c = 0;
                    countries.Add(
                        new Country
                        {
                            CountryCode = split[c++],
                            Latitude = double.Parse(split[c++].Replace('.', ',')),
                            Longitude = double.Parse(split[c++].Replace('.', ',')),
                            Name = split[c],
                        });
                }

                return countries;
            }
        }
    }
}