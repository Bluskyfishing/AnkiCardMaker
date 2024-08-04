using HtmlAgilityPack;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AnkiCardMaker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //UTF-8 for correct display of kanji.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            List<string[]> allKanjiList = new List<string[]>();
            int index = 0;
            
            try
            {
                while (true)
                {
                    //Menu//
                    if (allKanjiList.Count != 0)
                    {
                        Console.Write("Kanji to be added:");
                        foreach (string[] array in allKanjiList)
                        {
                            Console.Write(array[0] + ",");
                        }
                        Console.WriteLine("\n");
                    }
                    
                    Console.WriteLine("'x' to exit program.\n'w' to write to file.\n'b' to bulk add comma separated kanji text. (Without example sentences.)\nInput Kanji/Kanji word:");
                    string kanji = Console.ReadLine().Trim().ToLower();

                    if (kanji == "x") //exit 
                    {
                        Console.WriteLine("Program closed!");
                        break;
                    }

                    //if (allKanjiList.Contains(kanji)) //print notice that duplicate get index of kanji and overwrite.
                    //{
                    //    continue;
                    //}

                    else if (kanji == "w" && allKanjiList.Count > 0) //write to file 
                    {
                        FileWriteClass.writeToFile(allKanjiList);
                        break;
                    }

                    else if (kanji == "b") //bulk add kanji
                    {
                        Console.Clear();
                        Console.WriteLine("BULK MODE (Without example sentences.):\nInput Kanji/Kanji word separated by ',' ('x,y,z'):");

                        string csvKanjiInput = Console.ReadLine().ToLower();
                        string[] csvKanji = csvKanjiInput.Split(",");

                        foreach (string word in csvKanji)
                        {
                            string exampleSentence = " ";

                            List<string[]> data = kanjiLookupClass.kanjiLookup(word.Trim(),  allKanjiList);
                            
                            if (data.Count > 0) 
                            {
                                Console.WriteLine($"Added: {word.Trim()}");
                            }
                            
                        }

                        FileWriteClass.writeToFile(allKanjiList);
                        break;
                    }

                    //Checks if kanji exists and adds to list of total kanji.
                    List<string[]> kanjiInfo = kanjiLookupClass.kanjiLookup(kanji, allKanjiList);

                    if (kanjiInfo.Count != 0)
                    {
                        if (kanjiInfo[index].Length > 5)
                        {
                            Console.WriteLine("Write a sentence for the kanji:");
                            string sentence = Console.ReadLine().Trim();

                            kanjiInfo[index][1] = sentence;

                            index++;
                            Console.Clear() ;
                        }
                    }
                    else 
                    {
                        continue;
                    }
                    
                    //Note 直 shows old kanji. 
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR:" + ex.Message); }

            Console.ReadLine(); //stops console from closing.
        }
    }
}