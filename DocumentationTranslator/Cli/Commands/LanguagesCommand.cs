using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Cli.Commands
{
	public class LanguagesCommand
	{

		public static async void Run()
		{
			using var client = new HttpClient();

			client.BaseAddress = new Uri("https://localhost:7062/");

			try
			{
				HttpResponseMessage response = await client.GetAsync("api/Languages");

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();

					using (JsonDocument document = JsonDocument.Parse(responseBody))
					{
						JsonElement root = document.RootElement;

						if (root.ValueKind == JsonValueKind.Array)
						{
							foreach (JsonElement languageElement in root.EnumerateArray())
							{
								if (languageElement.TryGetProperty("languageName", out JsonElement languageValue))
								{
									string languageName = languageValue.GetString();
									Console.WriteLine(languageName);
								}
							}
						}
					}
				}
				else
				{
					Console.WriteLine("Failed to fetch data from the API.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}
	}
}
