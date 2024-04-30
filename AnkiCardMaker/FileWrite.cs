using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiCardMaker
{
    public class FileWriteClass
    {
        public static void writeToFile(List<string[]> kanjiInfo)
        {

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
    }
}
