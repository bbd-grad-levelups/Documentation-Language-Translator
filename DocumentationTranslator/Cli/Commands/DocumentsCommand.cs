using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Cli.Commands
{
    public class DocumentsCommand
    {

        public static async Task Run(string idToken)
        {
			using var client = new HttpClient();

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
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
								string doc = documentElement.GetString();
								string lang = languageElement.GetString();

								if (lang == "")
								{
									lang = "unspecified";
								}

								Console.WriteLine($"{doc} ({lang})");
							}
						}
						else
						{
							Console.WriteLine("Invalid JSON format.");
						}
					}


				}
				else
				{
					Console.WriteLine("Failed to fetch data from the API.");
					Console.WriteLine(response.StatusCode);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}
    }
}
