using Xunit;
using Cli.Commands;
using System;
using System.IO;

namespace Cli.Tests
{
	public class HelpCommandTests
	{
		[Fact]
		public void Run_PrintsAllCommands()
		{
			using (var sw = new StringWriter())
			{
				Console.SetOut(sw);
				HelpCommand.Run();

				string result = sw.ToString();
				foreach (var command in HelpCommand.commands)
				{
					Assert.Contains($"Command: {command.Name}", result);
					Assert.Contains($"Description: {command.Description}", result);
				}
			}
		}

		[Fact]
		public void CommandsList_NotEmpty()
		{
			Assert.NotEmpty(HelpCommand.commands);
		}

		[Fact]
		public void CommandsList_ContainsExpectedNumberOfCommands()
		{
			int expectedNumberOfCommands = 9;
			Assert.Equal(expectedNumberOfCommands, HelpCommand.commands.Count);
		}

		[Theory]
		[InlineData("help", "Displays list of available commands")]
		[InlineData("login", "Allows user to login with Google")]
		[InlineData("logout", "Logout of the application")]
		[InlineData("languages", "Lists all supported languages")]
		[InlineData("translate", "Upload a file (.txt/.md) to be translated \nrun command as follows: \u001b[33mtranslate <language> <filepath>\u001b[0m")]
		[InlineData("documents", "Returns list of previously translated documents")]
		[InlineData("download", "Downloads translated document to directory specified by user \nrun command as follows: \u001b[33mdownload <document_id> <directory_path>\u001b[0m")]
		[InlineData("clear", "Clears the console")]
		[InlineData("exit", "Closes the application")]
		public void CommandsList_ContainsExpectedCommandsWithDescriptions(string expectedName, string expectedDescription)
		{
			var command = HelpCommand.commands.Find(c => c.Name == expectedName);
			Assert.NotNull(command);
			Assert.Equal(expectedDescription, command.Description);
		}

		[Theory]
		[InlineData("notExistingCommand")]
		public void CommandsList_DoesNotContainUndefinedCommands(string commandName)
		{
			var command = HelpCommand.commands.Find(c => c.Name == commandName);
			Assert.Null(command);
		}
	}
}