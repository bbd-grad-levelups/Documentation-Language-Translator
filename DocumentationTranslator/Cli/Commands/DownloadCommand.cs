using System.Net.Http.Headers;
using System.Text.Json;

namespace Cli.Commands
{
	public class DownloadCommand
    {
        public static async Task Run(string idToken, string id, string filepath)
        {
			if (string.IsNullOrEmpty(filepath))
			{
				Console.WriteLine("\u001b[31mError: Filepath cannot be empty\u001b[0m");
				return;
			}

			if (!Directory.Exists(filepath))
			{
				Console.WriteLine("\u001b[31mError: Invalid or non-existant directory\u001b[0m");
				return;
			}

			if (Path.GetExtension(filepath) != "")
			{
				Console.WriteLine("\u001b[31mError: Filepath should point to a directory, not a file\u001b[0m");
				return;
			}

			using var client = new HttpClient();

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
			client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

			try
			{
				HttpResponseMessage response = await client.GetAsync($"api/document/{id}");

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					
					using (JsonDocument document = JsonDocument.Parse(responseBody))
					{
						JsonElement root = document.RootElement;

						JsonElement contentElement = root.GetProperty("documentContent");
						JsonElement titleElement = root.GetProperty("documentTitle");

						string content = contentElement.ToString();
						string title = titleElement.ToString();

						string fullFilePath = Path.Combine(filepath, $"{title}");
						File.WriteAllText(fullFilePath, content);

						Console.WriteLine($"\u001b[32mFile downloaded to: {fullFilePath}\u001b[0m");
					}
				}
				else
				{
					Console.WriteLine($"\u001b[31mFailed to fetch document with ID {id}. Status code: {response.StatusCode}\u001b[0m");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\u001b[31mError: {ex.Message}\u001b[0m");
			}
		}
    }
}
