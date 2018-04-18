using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFLabsGeonames
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://download.geonames.org/
            // топ 50 городов Беларус по населению
            // ID geonamesId, Название кириллица, название латиница, индексЮ, широта, долгота, население  (CSV, utf-8)


            DateTime end;
            DateTime start = DateTime.Now;
            Console.WriteLine("### Start Time: " + start.ToLongTimeString());
            Console.WriteLine();


            var geoTemplate = Geoname.ReadTemplate("Template/geonames_template.txt");
            var geoPostCodeTemplate = Geoname.ReadTemplate("Template/geonames_postal_template.txt");

            var geoData = Geoname.ReadGeoData("Data/BY_data.txt");
            var geoPostCodeData = Geoname.ReadGeoData("Data/BY_postcode.txt");
            //var geoData = Geoname.ReadGeoData("D://work//Data//ALL_data.txt", "BY");
            //var geoPostCodeData = Geoname.ReadGeoData("D://work//Data//ALL_postcode.txt", "BY");

            Console.WriteLine("### Geo data lines: " + geoData.Count);
            Console.WriteLine("### GeoPost data lines: " + geoPostCodeData.Count);

            var geoDataMap = Geoname.Collate(geoData, geoTemplate);
            var geoPostMap = Geoname.Collate(geoPostCodeData, geoPostCodeTemplate);

            // PPL	populated place	a city, town, village, or other agglomeration of buildings where people live and work (http://www.geonames.org/export/codes.html)
            geoDataMap = geoDataMap.Where(g => !g["population"].Equals("0") && g["country code"].Equals("BY") && g["feature class"].Equals("P") /*g["feature code"].Equals("PPL")*/) // PPL2-4 надо? / А-область
                .ToList(); //<Dictionary<string, string>>

            geoPostMap = geoPostMap.Where(p => p["country code"].Equals("BY")).ToList();

            // связываем по координатам - приходится округлять, т.к. разные знач - после 2 знака после зпт
            var geoBy50 = (from itemA in geoDataMap
                           from itemB in geoPostMap
                           where (
                           Math.Round(double.Parse(itemA["latitude"].Replace(".", ",")), 2) == Math.Round(double.Parse(itemB["latitude"].Replace(".", ",")), 2)
                           && Math.Round(double.Parse(itemA["longitude"].Replace(".", ",")), 2) == Math.Round(double.Parse(itemB["longitude"].Replace(".", ",")), 2)
                           && !string.IsNullOrEmpty(itemB["admin name2"])
                           )
                           select new
                           {
                               geonameId = itemA["geonameid"],
                               cyrilName = itemB["place name"],
                               latinName = itemA["name"],
                               postCode = itemB["postal code"],
                               _asciiname = itemA["asciiname"],
                               _admname2 = itemB["admin name2"],
                               latitude = itemA["latitude"],
                               longitude = itemA["longitude"],
                               _latitude = Math.Round(double.Parse(itemB["latitude"].Replace(".", ",")), 2),
                               _longitude = Math.Round(double.Parse(itemB["longitude"].Replace(".", ",")), 2),
                               population = itemA["population"],

                               _AB = itemA["asciiname"].Except(itemB["admin name2"]).ToList().Aggregate(new StringBuilder(), (i, j) => i.Append(j)),
                               _BA = itemB["admin name2"].Except(itemA["asciiname"]).ToList()


                           }
                        ).OrderByDescending(i => int.Parse(i.population)).Take(50).ToList();

            //save
            //using (StreamWriter sw = File.CreateText("BY_out.csv"))
            using (StreamWriter sw = new StreamWriter(File.Open("BY_out.csv", FileMode.Create), Encoding.UTF8))
            {
                foreach (var g in geoBy50)
                {
                    sw.WriteLine($"{g.geonameId}\t{g.cyrilName}\t{g.latinName}\t{g.postCode}\t{g.latitude}\t{g.longitude}\t{g.population}");
                }
            }

            end = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine("### End Time: " + end.ToLongTimeString());
            Console.WriteLine("### Run Time: " + (end - start));
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
