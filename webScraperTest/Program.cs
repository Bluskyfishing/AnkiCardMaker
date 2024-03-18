using HtmlAgilityPack;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace webScraperTest
{
    internal class Program
    {
        public static class HTMLParser
        {
            public static string getHTMLNode(HtmlDocument HTMLDocument, string nodeType, string xpath)
            {
                if (nodeType == "SelectSingleNode")
                {
                    var documentElement = HTMLDocument.DocumentNode.SelectSingleNode(xpath);
                    var documentElementValue = documentElement.InnerText.Trim();

                    return documentElementValue;
                }
                else if (nodeType == "SelectNodes")
                {
                    var documentElement = HTMLDocument.DocumentNode.SelectNodes(xpath);
                    List<string> elementNodes = new List<string>();

                    foreach (var node in documentElement)
                    {
                        elementNodes.Add(node.InnerHtml);
                    }

                    string strElementNodes = String.Join("|", elementNodes);

                    return strElementNodes;
                }
                else
                {
                    Console.WriteLine("Cant find Elements!");
                    return "NOT FOUND";
                } 

            }
        }

        public static void writeToFile(string JMdictID, string kanji, string furigana, string meanings, string tags)
        {

        }

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

                //Get JMdict ID 
                var JMdictIDNodes = htmlDocument.DocumentNode.SelectNodes("//ul[@class='f-dropdown']/li/a");
                List<char> JMdictIDList = new List<char>();

                foreach (var node in JMdictIDNodes)
                {
                    if (node.OuterHtml.StartsWith("<a href=\"https://www.edrdg.org"))
                    {
                        foreach (char c in node.OuterHtml.Substring(74))
                        {
                            if (!char.IsNumber(c)) { break; }
                            JMdictIDList.Add(c);
                        }

                    }

                }
                string JMdictID = string.Join("", JMdictIDList);
                Console.WriteLine($"JMdictID: {JMdictID}");

                //Get Furigana
                string furigana = HTMLParser.getHTMLNode(htmlDocument, "SelectSingleNode", "//span[@class='furigana']");
                Console.WriteLine($"Furigana: {furigana}");

                //Get Meaning(s)
                string meanings = HTMLParser.getHTMLNode(htmlDocument, "SelectNodes", "//span[@class='meaning-meaning']");

                Console.WriteLine($"Meanings: {meanings}");

                //Get Tags
                string tags = HTMLParser.getHTMLNode(htmlDocument, "SelectNodes", "//div[@class='meaning-tags']");

                Console.WriteLine($"Tags: {tags}");

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
                writeToFile(JMdictID, kanji, furigana, meanings, tags);
            }


        }
    }
}