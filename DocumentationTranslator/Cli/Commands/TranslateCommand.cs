using System;
using System.IO;

namespace frontend_cli.Commands
{
    internal class TranslateCommand
    {
        public static void Run(string filepath)
        {
            if (File.Exists(filepath))
            {
                Console.WriteLine("\u001b[32mValid file entered\u001b[0m");
                string fileContent = File.ReadAllText(filepath);
                Console.WriteLine(fileContent);
            }
            else
            {
                Console.WriteLine("\u001b[31mPlease upload a valid file\u001b[0m");
            }
        }
    }
}