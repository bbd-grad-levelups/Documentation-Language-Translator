using System.Net.Http.Headers;
using System.Text.Json;

namespace Cli.Commands
{
	public class LanguagesCommand
	{
		public static List<(string language, int id)> LanguageList { get; } = new List<(string language, int id)>();

		public static async Task InitLanguages(string idToken)
		{
			using var client = new HttpClient();

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
			client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

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
							foreach (JsonElement element in root.EnumerateArray())
							{
								JsonElement languageElement = element.GetProperty("language");
								string language = languageElement.GetString();
								JsonElement idElement = element.GetProperty("languageID");
								int id = idElement.GetInt32();
								
								LanguageList.Add((language, id));
							}
						}
						else
						{
							Console.WriteLine("\u001b[31mInvalid JSON format\u001b[0m");
						}
					}
				}
				else
				{
					Console.WriteLine($"\u001b[31mError: {response.StatusCode}\u001b[0m");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\u001b[31mError: {ex.Message}\u001b[0m");
			}
		}

		public static void Run()
		{
			if (LanguageList.Count == 0)
			{
				Console.WriteLine("Unable to retrieve supported languages");
			}
			else
			{
				foreach (var languageTuple in LanguageList)
				{
					Console.WriteLine(languageTuple.language);
				}
			}
		}
	}
}
