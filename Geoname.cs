using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HFLabsGeonames
{
    /// <summary>
    /// шаблон табл геонеймса
    /// </summary>
    class Geoname
    {
        /// <summary>
        /// чтение шаблона
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> ReadTemplate(string fileName)
        {
            char separator = ':';
            List<string> templList = new List<string>();
            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    templList.Add(s.Split(separator).First().Trim());
                }
            }

            return templList;
        }

        /// <summary>
        /// загрузка данных с предварит выборкой по стране
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contry"></param>
        /// <returns></returns>
        public static List<string[]> ReadGeoData(string fileName, string country = "BY")
        {
            char separator = '\t';
            List<string[]> geoList = new List<string[]>();

            var start = DateTime.Now;
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    var geoLine = s.Split(separator);
                    if (geoLine.Contains(country))
                        geoList.Add(geoLine);
                }
            }
            var end = DateTime.Now;
            //Console.WriteLine("Finished at: " + end.ToLongTimeString());
            //Console.WriteLine("Time: " + (end - start));
            //Console.WriteLine();

            GC.Collect();

            return geoList;
        }

        /// <summary>
        /// сопоставить данные и шаблон
        /// </summary>
        /// <param name="geoData"></param>
        /// <param name="geoTemplate"></param>
        /// <returns></returns>
        internal static List<Dictionary<string, string>> Collate(List<string[]> geoData, List<string> geoTemplate)
        {
            if (geoData == null || geoTemplate == null || geoData.Count == 0 || geoTemplate.Count == 0)
                throw new MissingFieldException("Отсутствуют данные или шаблон данных.");

            if (geoData.First().Count() != geoTemplate.Count)
                throw new MissingFieldException("Данные не соотв шаблону.");

            List<Dictionary<string, string>> geoMapList = new List<Dictionary<string, string>>();

            foreach (var geo in geoData)
            {
                var dic = geoTemplate.Zip(geo, (k, v) => new { key = k, value = v }).ToDictionary(x => x.key, x => x.value);
                geoMapList.Add(dic);
            }

            return geoMapList;
        }
    }
}
