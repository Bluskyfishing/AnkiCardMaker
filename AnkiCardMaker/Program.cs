using HtmlAgilityPack;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AnkiCardMaker
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
            //Console.WriteLine("Save as filename: "); //Input filename 
            //string input = Console.ReadLine().Trim();
            //string fileName = $"{input}.txt";
            //string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            string currentTimeFormated = DateTime.Now.ToString(@"HH-mm-dd-MM-yyyy"); //Time for filename. Skips the need for input of filename.
            string fileName = $"{currentTimeFormated}.txt";
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
                                writer.Write('"' + element + '"' + ";");
                            }
                            writer.Write('\n');
                        }
                    }
                    Console.WriteLine($"File created successfully!{filePath}\nWrote: {kanjiInfo.Count} Kanji to file.");
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        public static List<string[]> kanjiLookup(string kanji, List<string[]> allKanji)
        {
            string[] kanjiInfo = new string[5];

            kanjiInfo[0] = kanji;

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
                Console.WriteLine($"Unable to find kanji:{kanji}");
                return new List<string[]>();
            }

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
            kanjiInfo[1] = JMdictID;

            //Get Furigana
            string furigana = HTMLParser.getHTMLNode(htmlDocument, "SelectSingleNode", "//span[@class='furigana']");
            kanjiInfo[2] = furigana;

            //Get Meaning(s)
            var meaningsNodes = htmlDocument.DocumentNode.SelectNodes("//span[@class='meaning-meaning']");
            List<string> meaningsList = new List<string>();
            string[] meaningsBlackList = ["<s"];

            int num = 1;

            foreach (var node in meaningsNodes)
            {
                if (num > 5) { break; } //Limit of 5 meanings per kanji.
                if (meaningsBlackList.Contains(node.InnerHtml.Substring(0, 2))) { continue; }
                meaningsList.Add($"{num}. {node.InnerHtml}");
                num++;
            }
            string meanings = string.Join(" <br /> ", meaningsList);
            kanjiInfo[3] = meanings;

            //Get Tags 
            try
            {
                var tagsNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='meaning-tags']");
                HashSet<string> tagsList = new HashSet<string>();
                string[] tagsBlackList = ["Wikipedia definition", "verb-Other", "Other forms", "Notes"];

                foreach (var node in tagsNodes)
                {
                    if (tagsBlackList.Contains(node.InnerText)) { continue; }
                    if (node.InnerText.Length > 35)
                    {
                        if (node.InnerText.Substring(0, 11) == "Expressions")
                        {
                            string expressionString = node.InnerText.Substring(0, 10) + " " + node.InnerText.Substring(36) + " ";
                            string expression = expressionString.Replace(", ", "");

                            tagsList.Add(expression);
                            continue;
                        }

                    }

                    string[] tagSplit = node.InnerText.Split(", ");

                    for (int i = 0; i < tagSplit.Length; i++)
                    {
                        string noSpacetag = tagSplit[i].Replace(" ", "_"); //separates space with "_"
                        tagsList.Add(noSpacetag + " ");
                    }
                }

                string tagsString = string.Join("", tagsList);
                string tags = tagsString.Replace("&#39;", "'"); //fixes weird unicode bug.

                kanjiInfo[4] = tags;
            }
            catch (NullReferenceException e)   
            {
                Console.WriteLine($"Couldnt find Tags! Exeption: {e}\nKanji added without tags.");
                kanjiInfo[4] = "NOTAG";
            }

            //Check if everything is in list.
            if (kanjiInfo.Length == 5)
            {
                allKanji.Add(kanjiInfo);
            }
            else
            {
                Console.WriteLine("Not enough info to add!");
                return new List<string[]>();
            }
            return allKanji;

        }
        
        static void Main(string[] args)
        {
            //UTF-8 for correct display of kanji.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            List<string[]> allKanjiList = new List<string[]>();

            try
            {
                while (true)
                {
                    //Menu//
                    if (allKanjiList.Count != 0)
                    {
                        Console.WriteLine("\nKanji to be added:");
                        foreach (string[] array in allKanjiList)
                        {
                            Console.Write(array[0] + ", ");
                        }
                        Console.WriteLine("\n");
                    }
                    Console.WriteLine("'x' to exit program.\n'w' to write to file.\n'b' to bulk add comma separated kanji text.\nInput Kanji/Kanji word:");

                    String kanji = Console.ReadLine().Trim().ToLower();

                    if (kanji == "x") //exit 
                    {
                        Console.WriteLine("Program closed!");
                        break;
                    }

                    if (kanji == "w" && allKanjiList.Count > 0) //write to file 
                    {
                        writeToFile(allKanjiList);
                        break;
                    }

                    if (kanji == "b") //bulk add kanji
                    {
                        Console.Clear();
                        Console.WriteLine("BULK MODE:\nInput Kanji/Kanji word separated by 、 ('x,y,z'):");

                        string csvKanjiInput = Console.ReadLine().ToLower();
                        string[] csvKanji = csvKanjiInput.Split("、");

                        foreach (string word in csvKanji)
                        {
                            List<string[]> data = kanjiLookup(word.Trim(), allKanjiList);
                            
                            if (data.Count > 0) 
                            {
                                Console.WriteLine($"Added: {word.Trim()}");
                            }
                            
                        }

                        writeToFile(allKanjiList);
                        break;
                    }

                    kanjiLookup(kanji, allKanjiList);

                    //Note 直 shows old kanji. 
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR:" + ex.Message); }

            Console.ReadLine(); //stops console from closing.
        }
    }
}