using HtmlAgilityPack;
using System.Dynamic;
using System.Text;

namespace webScraperTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //UTF-8 for correct display of kanji.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            while (true)
            {
                //Kanji search.
                Console.WriteLine("'stop' to exit program\nInput Kanji/Kanji word:");
                String kanji = Console.ReadLine().Trim();

                if (kanji == "stop")
                {
                    break;
                }

                //Get request jisho.org
                string kanjiURL = $"https://jisho.org/word/{kanji}";
                Console.WriteLine(kanjiURL);
                var httpClient = new HttpClient();
                var hmtl = httpClient.GetStringAsync(kanjiURL).Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(hmtl);

                //Get Furigana
                var furiganaElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='furigana']");
                var furigana = furiganaElement.InnerText.Trim();

                Console.WriteLine($"Furigana: {furigana}");

                //Get Meaning(s)
                var meaningNodes = htmlDocument.DocumentNode.SelectNodes("//span[@class='meaning-meaning']");
                int meaningNum = 1;

                foreach (var node in meaningNodes)
                {
                    Console.WriteLine($"Meaning: {meaningNum}. {node.InnerHtml}\n");
                    meaningNum++;
                }

                //Get Tags
                var tagNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='meaning-tags']");
                int tagNum = 1;

                foreach (var node in tagNodes)
                {
                    Console.WriteLine($"Tags: {tagNum}. {node.InnerHtml}\n");
                    tagNum++;
                }

                //Get JMdict ID 
                //example request: <a href="https://www.edrdg.org/jmwsgi/edform.py?svc=jmdict&amp;sid=&amp;q=1316240&amp;a=2">Edit in JMdict</a>

                var JMdictIDNodes = htmlDocument.DocumentNode.SelectNodes("//ul[@class='f-dropdown']/li/a");
                List<char> JMdictIDSet = new List<char>();

                foreach (var node in JMdictIDNodes) 
                {
                    if (node.OuterHtml.StartsWith("<a href=\"https://www.edrdg.org"))
                    {
                        //Console.WriteLine(node.OuterHtml);
                        //Console.WriteLine(node.OuterHtml.Substring(74));
                        //Console.WriteLine(node.OuterLength);

                        foreach (char c in node.OuterHtml.Substring(74))
                        {
                            if (!char.IsNumber(c)) { break; }
                            JMdictIDSet.Add(c);
                        }
                       
                    }

                }
                Console.WriteLine("JMdictID: " + string.Join("", JMdictIDSet));

                //Get example sentence
                //https://tatoeba.org/en/sentences/search?from=jpn&query=%E6%84%9F%E6%83%85&to=eng&page=2
                /*
                int page = 1;

                string sentenceURL = $"https://tatoeba.org/en/sentences/search?from=jpn&query={kanji}&to=eng&page={page}";
                var httpClient2 = new HttpClient();
                var hmtl2 = httpClient2.GetStringAsync(sentenceURL).Result;
                var htmlDocument2 = new HtmlDocument();
                htmlDocument2.LoadHtml(hmtl2);

                var sentenceNodes = htmlDocument2.DocumentNode.SelectNodes("//span[@class='layout-align-start-center layout-row flex']");

                foreach (var node in sentenceNodes)
                {
                    if (node == null)
                    {
                        Console.WriteLine("Null!");
                        continue;
                    }
                    Console.WriteLine($"Sentence: {node.InnerHtml}");
                    meaningNum++;
                }
                */

                //Get Pitch accent

            }


        }
    }
}