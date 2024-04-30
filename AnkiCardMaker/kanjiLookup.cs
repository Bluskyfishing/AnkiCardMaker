using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiCardMaker
{
    public class kanjiLookupClass
    {
        public static List<string[]> kanjiLookup(string kanji,string sentence, List<string[]> allKanji)
        {
            string[] kanjiInfo = new string[6];

            kanjiInfo[0] = kanji;

            kanjiInfo[1] = sentence;

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
            kanjiInfo[2] = JMdictID;

            //Get Furigana
            var furiganaNode = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='furigana']");
            string furigana = furiganaNode.InnerText.Trim();
            kanjiInfo[3] = furigana;

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
            kanjiInfo[4] = meanings;

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

                kanjiInfo[5] = tags;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($"Couldnt find Tags! Exeption: {e}\nKanji added without tags.");
                kanjiInfo[5] = "NOTAG";
            }

            //Check if everything is in list.
            if (kanjiInfo.Length == 6)
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
    }
}
