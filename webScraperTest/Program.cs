using HtmlAgilityPack;
using System.Dynamic;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

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
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (string[] array in kanjiInfo)
                        {
                            //array = [kanji, JMdictID, furigana, meanings, tags]
                            foreach (string element in array)
                            {
                                if (element == array[3])
                                {
                                    string breakElement = element.Replace("-", " <br /> "); //splits up meanings.
                                    writer.Write('"' + breakElement + '"' + ";"); 
                                    continue;
                                }
                                else if (element == array[4])
                                {
                                    string spacedElement = element.Replace(",.", " "); //spaces for tags in anki.
                                    string removedCommaElement = element.Replace(',', ' '); //to not get tags with ',' and without. For tag consistancy.
                                    writer.Write('"' + removedCommaElement + '"' + ";"); //";" to autofill tags when importing:
                                    continue;
                                }
                                writer.Write('"' + element + '"' + ";");
                            }
                            writer.Write('\n');
                        }
                    }
                    Console.WriteLine("File created successfully! " + filePath);
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

                //Menu//
                if (allKanjiList.Count != 0 ) 
                {
                    Console.WriteLine("Kanji to be added:");
                    foreach (string[] array in allKanjiList)
                    {
                        Console.Write(array[0] + ", ");
                    }
                    Console.WriteLine("\n");
                }
                Console.WriteLine("'x' to exit program\n'a' to add all kanji!\nInput Kanji/Kanji word:");
                String kanji = Console.ReadLine().Trim().ToLower();

                if (kanji == "x")
                {
                    Console.WriteLine("Program closed!");
                    break;
                }

                if (kanji == "a" && allKanjiList.Count > 0)
                {
                    writeToFile(allKanjiList);
                    break;
                }

                kanjiArray[0] = kanji;

                //Kanji search
                //Get request jisho.org
                var httpClient = new HttpClient();
                var htmlDocument = new HtmlDocument();
                
                try 
                {
                    string kanjiURL = $"https://jisho.org/word/{kanji}";
                    var hmtl = httpClient.GetStringAsync(kanjiURL).Result;
                    htmlDocument.LoadHtml(hmtl);
                }
                catch (AggregateException)
                {
                    Console.Clear();
                    Console.WriteLine("Unable to find kanji! Please Try again!");
                    continue;
                }
                Console.Clear();

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
                
                //Get Furigana
                string furigana = HTMLParser.getHTMLNode(htmlDocument, "SelectSingleNode", "//span[@class='furigana']");
                kanjiArray[2] = furigana;

                //Get Meaning(s)
                var meaningsNodes = htmlDocument.DocumentNode.SelectNodes("//span[@class='meaning-meaning']");
                
                List<string> meaningsList = new List<string>();
                string[] meaningsBlackList = ["<spa"];

                int num = 1;

                foreach (var node in meaningsNodes)
                {
                    if (meaningsBlackList.Contains(node.InnerHtml.Substring(0,4))) { continue; }
                    meaningsList.Add($"{num}. {node.InnerHtml}");
                    num++;
                }
                string meanings = String.Join("-", meaningsList);
                kanjiArray[3] = meanings;

                //Get Tags
                var tagsNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='meaning-tags']");
                
                List<string> tagsList = new List<string>();
                string[] tagsBlackList = ["Wikipedia definition","verb-Other", "Other forms", "Notes"];

                foreach (var node in tagsNodes)
                {
                    if (tagsBlackList.Contains(node.InnerText)) { continue; }
                    tagsList.Add(node.InnerText + ",");
                }

                string strTagList = String.Join("", tagsList); //separates different tags with "-".
                string sentencTags = strTagList.Replace(" ", "."); //Connects tag-sentences for import. Prevents generation of split tags.
                string tags = sentencTags.Replace("&#39;", "'"); //fixes weird unicode bug.

                kanjiArray[4] = tags;

                //print of array:
                //foreach (string s in kanjiArray) { Console.WriteLine($"kanjiArray:{s}"); }

                //Check if everything is in list.
                if (kanjiArray.Length == 5)
                {
                    allKanjiList.Add(kanjiArray);
                }
                else 
                { 
                    Console.WriteLine("Not enough info to add!"); 
                }
                
            }


        }
    }
}