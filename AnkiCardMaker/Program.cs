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
                    string kanji = Console.ReadLine().Trim().ToLower();

                    if (kanji == "x") //exit 
                    {
                        Console.WriteLine("Program closed!");
                        break;
                    }

                    else if (kanji == "w" && allKanjiList.Count > 0) //write to file 
                    {
                        FileWriteClass.writeToFile(allKanjiList);
                        break;
                    }

                    else if (kanji == "b") //bulk add kanji
                    {
                        Console.Clear();
                        Console.WriteLine("BULK MODE:\nInput Kanji/Kanji word separated by , ('x,y,z'):");

                        string csvKanjiInput = Console.ReadLine().ToLower();
                        string[] csvKanji = csvKanjiInput.Split(",");

                        foreach (string word in csvKanji)
                        {
                            string exampleSentence = " ";

                            List<string[]> data = kanjiLookupClass.kanjiLookup(word.Trim(), exampleSentence, allKanjiList);
                            
                            if (data.Count > 0) 
                            {
                                Console.WriteLine($"Added: {word.Trim()}");
                            }
                            
                        }

                        FileWriteClass.writeToFile(allKanjiList);
                        break;
                    }

                    Console.WriteLine("Write a sentence for the kanji:");
                    string sentence = Console.ReadLine();

                    kanjiLookupClass.kanjiLookup(kanji, sentence, allKanjiList);

                    //Note 直 shows old kanji. 
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR:" + ex.Message); }

            Console.ReadLine(); //stops console from closing.
        }
    }
}