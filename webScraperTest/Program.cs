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
                Console.WriteLine("Input Kanji/Kanji word:");
                String kanji = Console.ReadLine();

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

                //Get JMdict ID. 
            }


        }
    }
}