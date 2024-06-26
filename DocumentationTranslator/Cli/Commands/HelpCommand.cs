﻿namespace Cli.Commands
{
	public class HelpCommand
    {
        public static List<CommandInfo> commands = new List<CommandInfo>()
        {
            new CommandInfo { Name = "help", Description = "Displays list of available commands" },
            new CommandInfo { Name = "login", Description = "Allows user to login with Google" },
            new CommandInfo { Name = "logout", Description = "Logout of the application" },
			new CommandInfo { Name = "languages", Description = "Lists all supported languages" },
			new CommandInfo { Name = "translate", Description = "Upload a file (.txt/.md) to be translated \nrun command as follows: \u001b[33mtranslate <language> <filepath>\u001b[0m" },
            new CommandInfo { Name = "documents", Description = "Returns list of previously translated documents" },
            new CommandInfo { Name = "download", Description = "Downloads translated document to directory specified by user \nrun command as follows: \u001b[33mdownload <document_id> <directory_path>\u001b[0m" },
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
