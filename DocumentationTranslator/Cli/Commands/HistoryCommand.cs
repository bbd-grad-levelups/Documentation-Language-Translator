using System;
using System.Collections.Generic;

namespace frontend_cli.Commands
{
    public class HistoryCommand
    {
        private static List<string> history = new List<string>();

        public static void Run()
        {

            history.Add("file_1");
            history.Add("file_2");

            foreach (var file in history)
            {
                Console.WriteLine($"filename: {file}");
            }
        }
    }
}
