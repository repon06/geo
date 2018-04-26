using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HFLabsGeonames
{
    class ReadFile
    {

        public static int GetCountLines(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            Console.WriteLine($"### Load file: {info.Name} / {info.Length} byte");

            int result = 0;
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                    result++;
            }
            GC.Collect();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="searhStr"></param>
        /// <param name="caseSensitive">учитывать регистр</param>
        /// <returns></returns>
        public static int GetCountStr(string fileName, string searhStr, bool caseSensitive = true)
        {
            FileInfo info = new FileInfo(fileName);
            Console.WriteLine($"### Load file: {info.Name} / {info.Length} byte");

            int result = 0;
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(searhStr))
                    {
                        int count = (s.Length - (caseSensitive ? s.Replace(searhStr, string.Empty).Length : s.ToLower().Replace(searhStr.ToLower(), string.Empty).Length)) / searhStr.Length;
                        result += count;
                    }
                }
            }
            GC.Collect();
            return result;
        }

        public static int GetCountStrRegex(string fileName, string searhStr, bool caseSensitive = true)
        {
            FileInfo info = new FileInfo(fileName);
            Console.WriteLine($"### Load file: {info.Name} / {info.Length} byte");

            int result = 0;
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(searhStr))
                    {
                        int count = new Regex(searhStr).Matches(s).Count;
                        result += count;
                    }
                }
            }
            GC.Collect();
            return result;
        }

    }
}
