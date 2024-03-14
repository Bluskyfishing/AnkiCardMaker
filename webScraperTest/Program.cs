using HtmlAgilityPack;
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

            //Kanji to search for:
            Console.WriteLine("Input Kanji/Kanji word:");
            String kanji = Console.ReadLine();

            //Get request jisho.org
            Console.WriteLine(kanji);
            string url = $"https://jisho.org/word/{kanji}";
            Console.WriteLine(url);
            var httpClient = new HttpClient();
            var hmtl = httpClient.GetStringAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(hmtl);

            //Get furigana
            var furiganaElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='furigana']");
            var furigana = furiganaElement.InnerText.Trim();
            Console.WriteLine(furigana);

            //Get meaning 
            //var meaningElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='meaning-meaning']");
            //var meaning = meaningElement.InnerText.Trim();
            //Console.WriteLine(meaning);

            var meaningNodes = htmlDocument.DocumentNode.SelectNodes("//span[@class='meaning-meaning']");

            foreach( var node in meaningNodes)
            {
                Console.WriteLine(node.InnerHtml);
                Console.WriteLine();
            }

        }
    }
}