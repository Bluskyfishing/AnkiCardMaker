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
                string url = $"https://jisho.org/word/{kanji}";
                Console.WriteLine(url);
                var httpClient = new HttpClient();
                var hmtl = httpClient.GetStringAsync(url).Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(hmtl);

                //Get furigana
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

                //Get tags
                var tagNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='meaning-tags']");
                int tagNum = 1;

                foreach (var node in tagNodes)
                {
                    Console.WriteLine($"Tags: {tagNum}. {node.InnerHtml}\n");
                    tagNum++;
                }


                //Get example sentence
                Console.WriteLine("Example Sentence");

                //Get JMdict ID 

                var JMdictIDNodes = htmlDocument.DocumentNode.SelectNodes("//ul[@class='f-dropdown']/li/a");

                //example request: <a href="https://www.edrdg.org/jmwsgi/edform.py?svc=jmdict&amp;sid=&amp;q=1316240&amp;a=2">Edit in JMdict</a>

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


                //Get Pitch accent
            }


        }
    }
}