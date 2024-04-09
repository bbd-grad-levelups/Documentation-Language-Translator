using System;
using Xunit;
using System.IO;
using Cli.Commands;

namespace CliTests
{
	public class ClearCommandTests
	{
		[Fact]
		public void Run_ClearsConsole()
		{
			var output = new StringWriter();
			Console.SetOut(output);

			ClearCommand.Run();

			Assert.Equal(string.Empty, output.ToString().Trim());
		}

		[Fact]
		public void Run_ShouldNotThrowException()
		{
			var exception = Record.Exception(() => ClearCommand.Run());
			Assert.Null(exception);
		}
	}
}
