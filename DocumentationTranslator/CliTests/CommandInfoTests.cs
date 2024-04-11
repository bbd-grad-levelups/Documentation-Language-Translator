using Xunit;
using Cli.Commands;

namespace Cli.Tests
{
	public class CommandInfoTests
	{
		[Fact]
		public void CommandInfo_PropertiesAreSet()
		{
			var commandInfo = new CommandInfo();
			var name = "TestCommand";
			var description = "This is a test command.";

			commandInfo.Name = name;
			commandInfo.Description = description;

			Assert.Equal(name, commandInfo.Name);
			Assert.Equal(description, commandInfo.Description);
		}
	}
}
