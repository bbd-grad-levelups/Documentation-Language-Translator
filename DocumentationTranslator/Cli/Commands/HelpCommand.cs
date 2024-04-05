using System;
using System.Collections.Generic;

namespace frontend_cli.Commands
{
    public class HelpCommand
    {
        public static List<CommandInfo> commands = new List<CommandInfo>()
        {
            new CommandInfo { Name = "help", Description = "Displays list of available commands" },
            new CommandInfo { Name = "login", Description = "Allows user to login with Google" },
            new CommandInfo { Name = "logout", Description = "Logout of the application" },
            new CommandInfo { Name = "translate", Description = "Upload a file to be translated \nrun command as follows: \u001b[33mtranslate <filepath>\u001b[0m" },
            new CommandInfo { Name = "history", Description = "Returns list of previously translated documents" },
            new CommandInfo { Name = "download", Description = "Downloads translated document to filepath specified by user \nrun command as follows: \u001b[33mdownload <filename> <filepath>\u001b[0m" },
            new CommandInfo { Name = "clear", Description = "Clears the console" },
            new CommandInfo { Name = "exit", Description = "Closes the application" },
        };

        public static void Run()
        {
            foreach (var command in commands)
            {
                Console.WriteLine("\n");
                Console.WriteLine($"Command: {command.Name}");
                Console.WriteLine($"Description: {command.Description}");
                Console.WriteLine("--------------------------------------------------------------------");
            }
            Console.WriteLine("\n");
        }
    }
}
