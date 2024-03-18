using HtmlAgilityPack;
using System.Dynamic;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

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
                else
                {
                    return "NOT FOUND";
                } 

            }
        }

        public static void writeToFile(List<string[]> kanjiInfo)
        {
            Console.WriteLine("Save as filename: ");
            string input = Console.ReadLine().Trim();

            string fileName = $"{input}.txt";
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            
            try
            {
                using (FileStream fs = File.Create(filePath))
                {
                    Console.WriteLine("File created successfully! " + filePath);

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (string[] item in kanjiInfo)
                        {
                            writer.Write(item + ";");
                        }
                    }

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        static void Main(string[] args)
        {
            //UTF-8 for correct display of kanji.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            List<string[]> allKanjiList = new List<string[]>();

            while (true)
            {
                string[] kanjiArray = new string[5];

                //Kanji search.
                Console.WriteLine("'x' to exit program\nInput Kanji/Kanji word:");
                String kanji = Console.ReadLine().Trim();

                if (kanji == "x")
                {
                    break;
                }

                if (kanji == "add" && allKanjiList.Count > 0)
                {
                    writeToFile(allKanjiList);
                }

                kanjiArray[0] = kanji;

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
                kanjiArray[1] = JMdictID;

                Console.WriteLine($"JMdictID: {JMdictID}");

                //Get Furigana
                string furigana = HTMLParser.getHTMLNode(htmlDocument, "SelectSingleNode", "//span[@class='furigana']");

                Console.WriteLine($"Furigana: {furigana}");
                kanjiArray[2] = furigana;

                //Get Meaning(s)
                var meaningsNodes = htmlDocument.DocumentNode.SelectNodes("//span[@class='meaning-meaning']");
                List<string> meaningsList = new List<string>();

                foreach (var node in meaningsNodes)
                {
                    meaningsList.Add(node.InnerHtml);
                }
                string meanings = String.Join("|", meaningsList);
                kanjiArray[3] = meanings;

                Console.WriteLine($"Meanins: {meanings}");

                //Get Tags
                var tagsNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='meaning-tags']");
                List<string> tagsList = new List<string>();
                string[] tagsBlackList = ["Wikipedia definition"];

                foreach (var node in tagsNodes)
                {
                    if (tagsBlackList.Contains(node.InnerHtml)) { continue; }
                    tagsList.Add(node.InnerHtml);
                }
                string tags= String.Join("|", tagsList);
                kanjiArray[4] = tags;

                Console.WriteLine($"Tags: {tags}");

                foreach (string s in kanjiArray) { Console.WriteLine($"kanjiArray:{s}"); }

                if (kanjiArray.Length == 5)
                {
                    allKanjiList.Add(kanjiArray);
                }
                else 
                { 
                    Console.WriteLine("Not enough info to add!"); 
                }
                
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
            }


        }
    }
}