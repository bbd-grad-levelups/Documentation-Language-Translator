using System.Net.Http.Headers;
using System.Text.Json;
using DocTranslatorServer.Models;

namespace WebApp.Components.Pages;

public partial class Home
{

	// Web UI code:
	public string? fileContent { get; set; }
	public string? newDocumentName;
	public string messageInfo = "Provide valid input";
	public List<Languages> languages = new List<Languages> { };
	public List<Document> Documents = new List<Document> { };
	public string? inputLanguage, outputLanguage;
	public int maxFileSizeBytes = 1024, documentID = -1;

	// Functions to call the API:
	private async Task<List<Languages>> getLanguages()
	{
		using var client = new HttpClient();

		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
		client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

		var response = await client.GetAsync("api/languages");
		string responseBody = await response.Content.ReadAsStringAsync();

		var result = new List<Languages> { };

		using(JsonDocument document = JsonDocument.Parse(responseBody))
		{
			JsonElement root = document.RootElement;

			foreach (JsonElement currLang in root.EnumerateArray())
			{
				var tempLang = new Languages()
				{
					LanguageID = int.Parse(currLang.GetProperty("languageID").ToString()),
					Language = currLang.GetProperty("language").ToString(),
					Abbreviation = currLang.GetProperty("abbreviation").ToString()
				};
				result.Add(tempLang);
			}
		}
		
		return result;
	}

	private async Task<List<Document>> getDocumentNames()
	{
		using var client = new HttpClient();

		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
		client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

		var response = await client.GetAsync("api/document/names");
		string responseBody = await response.Content.ReadAsStringAsync();

		var result = new List<Document> { };

		using(JsonDocument document = JsonDocument.Parse(responseBody))
		{
			JsonElement root = document.RootElement;

			foreach(JsonElement currDoc in root.EnumerateArray())
			{
				var tempDoc = new Document()
				{
					DocumentID = int.Parse(currDoc.GetProperty("documentID").ToString()),
					DocumentName = currDoc.GetProperty("documentName").ToString()
				};

				result.Add(tempDoc);
			}
		}

		return result;
	}

	private async void UploadDocument()
	{
		var mustUpload = true;
		if(String.IsNullOrEmpty(newDocumentName))
		{
			// Tell the user that their docname is invalid
			messageInfo = "Please give the new document a name.";
			showPopup = true;
			mustUpload = false;
			return;
		}
		if(newDocumentName.Length > 15) 
		{
			// Tell the user that their docname is invalid
			messageInfo = "Please give the new document a name within 15 charactrs.";
			showPopup = true;
			mustUpload = false;
			return;
		}
		if(String.IsNullOrEmpty(fileContent))
		{
			// Tell the user that they must enter file content
			messageInfo = "Please enter file contents.";
			showPopup = true;
			mustUpload = false;
			return;
		}
		if(fileContent.Length > maxFileSizeBytes)
		{
			// Tell the user that their file was too long
			messageInfo = "File too big.";
			showPopup = true;
			mustUpload = false;
			return;
		}
		if(inputLanguage == outputLanguage)
		{
			// Tell the user that theey must select 2 different languages
			messageInfo = "Select different input and output languages.";
			showPopup = true;
			mustUpload = false;
			return;
		}
		if (!(newDocumentName.EndsWith(".md") || newDocumentName.EndsWith(".txt")))
		{
			messageInfo = "Make sure you upload a .txt or .md file ONLY";
			showPopup = true;
			mustUpload = false;
		}
		if(mustUpload)
		{
			var client = new HttpClient();

			var request = new HttpRequestMessage(HttpMethod.Post,
				"http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/api/document");
			request.Headers.Add("Authorization", idToken);

			var content = new
			{
				languageID = outputLanguage,
				documentTitle = newDocumentName,
				documentContent = fileContent
			};

			request.Content = new StringContent(JsonSerializer.Serialize(content), null, "application/json");


			var response = await client.SendAsync(request);

			if(response.IsSuccessStatusCode)
			{
				string responseBody = await response.Content.ReadAsStringAsync();

				using(JsonDocument document = JsonDocument.Parse(responseBody))
				{
					JsonElement root = document.RootElement;

					JsonElement titleElement = root.GetProperty("documentTitle");
					string docTitle = titleElement.ToString();

					messageInfo = $"Successfully uploaded {docTitle}";
					showPopup = true;
				}
				
				SetUpDropDowns();
			}
			else
			{
				Console.WriteLine($"Error: {response.StatusCode}\u001b[0m");
			}
		}
		
		SetUpDropDowns();
	}

	private async void ViewFile()
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
		client.BaseAddress = new Uri("http://doc-translator-env.eba-egxmirhg.eu-west-1.elasticbeanstalk.com/");

		var response = await client.GetAsync($"api/document/{documentID}");
		string responseBody = await response.Content.ReadAsStringAsync();

		using(JsonDocument document = JsonDocument.Parse(responseBody))
		{
			JsonElement root = document.RootElement;

			JsonElement documentContent = root.GetProperty("documentContent");
			fileContent = documentContent.ToString();
			StateHasChanged();
		}
	}
}
