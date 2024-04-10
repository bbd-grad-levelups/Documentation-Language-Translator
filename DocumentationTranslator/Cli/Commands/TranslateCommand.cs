using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Cli.Commands
{
    internal class TranslateCommand
    {
        public static async Task Run(string idToken, string language, string filepath)
        {
            if ( LanguagesCommand.LanguageList.Count == 0)
            {
                Console.WriteLine("Unable to retrieve supported languages");
                return;
            }

			var languageTuple = LanguagesCommand.LanguageList.FirstOrDefault(lang => lang.language.Equals(language, StringComparison.OrdinalIgnoreCase));

            if (languageTuple == default)
            {
                Console.WriteLine($"\u001b[31mLanguage '{language}' not supported\u001b[0m");
                return;
            }

            int languageId = languageTuple.id;

			if (File.Exists(filepath))
            {
                string extension = Path.GetExtension(filepath);

                if (!IsValidExtension(extension))
                {
					Console.WriteLine("\u001b[31mFile extension not supported. Please use .txt or .md files.\u001b[0m");
					return;
				}

				FileInfo fileInfo = new FileInfo(filepath);

				if (fileInfo.Length > 1000)
				{
					Console.WriteLine("\u001b[31mFile exceeds 1000 characters limit.\u001b[0m");
					return;
				}

                string fileContent = File.ReadAllText(filepath);
                string fileTitle = Path.GetFileNameWithoutExtension(filepath);

				var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, "http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/api/document");
                request.Headers.Add("Authorization", idToken);

				var content = new StringContent($"{{\"languageID\":{languageId},\"documentTitle\":\"{fileTitle}\",\"documentContent\":\"{fileContent}\"}}", null, "application/json");

                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
					string responseBody = await response.Content.ReadAsStringAsync();

					using (JsonDocument document = JsonDocument.Parse(responseBody))
					{
						JsonElement root = document.RootElement;

						JsonElement contentElement = root.GetProperty("documentContent");
						JsonElement titleElement = root.GetProperty("documentTitle");

						string docContent = contentElement.ToString();
						string docTitle = titleElement.ToString();

                        Console.WriteLine($"Translated document saved to database with title: {docTitle}");
                        Console.WriteLine("Translated text:");
                        Console.WriteLine(docContent);
                        Console.WriteLine("\nDownload document locally by using the `download` command");
					}
				}
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}\u001b[0m");
                }
			}
            else
            {
                Console.WriteLine("\u001b[31mPlease upload a valid file\u001b[0m");
			}
        }

        private static bool IsValidExtension(string extension)
        {
            return extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".md", StringComparison.OrdinalIgnoreCase);
        }
    }
}