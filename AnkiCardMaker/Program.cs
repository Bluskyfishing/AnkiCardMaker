using HtmlAgilityPack;
using System;
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

                            List<string[]> data = kanjiLookupClass.kanjiLookup(word.Trim(),  allKanjiList);
                            
                            if (data.Count > 0) 
                            {
                                Console.WriteLine($"Added: {word.Trim()}");
                            }
                            
                        }

                        FileWriteClass.writeToFile(allKanjiList);
                        break;
                    }

                    //Check for duplicate kanjis
                    bool kanjiDuplicate = false;

                    for (int i = 0; i < allKanjiList.Count; i++)
                    {

                        if (allKanjiList[i][0] == kanji)
                        {
                            kanjiDuplicate = true;

                            Console.WriteLine("Duplicate kanji found! Overwrite? (y/n): ");
                            string answer = Console.ReadLine().Trim().ToLower();

                            if (answer == "y") 
                            {
                                Console.WriteLine("Write a sentence for the kanji:");
                                string sentence = Console.ReadLine().Trim();

                                allKanjiList[i][1] = sentence;
                                kanjiDuplicate = true;

                                Console.WriteLine($"'{kanji}' updated with new sentence: {sentence}");
                                Thread.Sleep(2000);
                                Console.Clear();
                            }
                            else if (answer == "n")
                            {
                                Console.WriteLine($"Overwrite of '{kanji}' skipped!");
                                Thread.Sleep(2000);
                                Console.Clear();
                            }

                            break;
                        }
                    }

                    if (!kanjiDuplicate)
                    {
                        //Check if kanji exists and adds to list of total kanji.
                        List<string[]> kanjiInfo = kanjiLookupClass.kanjiLookup(kanji, allKanjiList);

                        if (kanjiInfo.Count > 0) 
                        {
                            if (kanjiInfo[0].Length > 5)
                            {
                                Console.WriteLine("Write a sentence for the kanji:");
                                string sentence = Console.ReadLine().Trim();

                                kanjiInfo[0][1] = sentence;
                                Console.Clear();
                            }
                        }
                        
                    }
                    
                    
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR:" + ex.Message); }

            Console.ReadLine(); //stops console from closing.
        }
    }
}