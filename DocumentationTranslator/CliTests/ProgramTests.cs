using System;
using Cli;
using Cli.Commands;
using Xunit;

namespace CliTests
{
	public class ProgramTests
	{
		[Fact]
		public void HelpCommandTest()
		{
			var writer = new StringWriter();
			Console.SetOut(writer);

			HelpCommand.Run();

			var output = writer.GetStringBuilder().ToString().Trim();
			Assert.NotEmpty(output);
		}
	}
}
