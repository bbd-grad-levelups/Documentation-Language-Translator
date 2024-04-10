using System.Net.Http.Headers;
using System.Text.Json;

namespace Cli.Commands
{
	public class DocumentsCommand
    {
        public static async Task Run(string idToken)
        {
			using var client = new HttpClient();

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
			client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

			try
			{
				HttpResponseMessage response = await client.GetAsync("api/document/names");

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
								JsonElement documentElement = element.GetProperty("documentName");
								JsonElement languageElement = element.GetProperty("language");
								JsonElement idElement = element.GetProperty("documentID");
								string doc = documentElement.GetString();
								string lang = languageElement.GetString();

								if (lang == "")
								{
									lang = "unspecified";
								}

								Console.WriteLine($"Name: {doc} | Language: {lang} | ID: {idElement}");
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
    }
}
