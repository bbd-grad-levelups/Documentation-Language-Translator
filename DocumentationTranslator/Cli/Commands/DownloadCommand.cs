using System;
using System.IO;

namespace Cli.Commands
{
    internal class DownloadCommand
    {
        public static void Run(string fileToDownload, string filepath)
        {
            string dummyFile = "Super random text";

            try
            {
                using (StreamWriter writer = new StreamWriter(filepath))
                {
                    writer.Write(dummyFile);
                }

                Console.WriteLine("\u001b[32mFile downloaded successfully\u001b[0m");
            }
            catch (IOException e)
            {
                Console.WriteLine("\u001b[31mAn error has occurred while downloading the file: " + e.Message + "\u001b[0m");
            }

        }
    }
}
