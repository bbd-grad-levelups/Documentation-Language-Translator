using System.Net.Http.Headers;
using System.Text.Json;
using DocTranslatorServer.Models;

namespace WebApp.Components.Pages;

public partial class Home
{
	// OAuth:
	private static string scopes = "profile email";
	private static string redirectUri = "";
	private static string clientId = "";
	private static string clientSecret = "";
	private string code = "";
	private static string accessToken = "";
	private static string idToken = "";

	protected override async Task OnInitializedAsync()
	{
		// OAuth stuff:
		if(clientId == "")
		{
			ReadSecrets();

			if(clientId != "")
			{
				string url = await GetCode();

				Console.WriteLine(url);

				NavigationManager.NavigateTo(url);

				await Task.Delay(5000);
			}
		}
		else if(clientId != "" && idToken == "")
		{
			var uri = new Uri(NavigationManager.Uri);
			var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

			if(queryParams["code"] != null)
			{
				code = queryParams["code"];

				//Console.WriteLine("This is the auth code:");
				//Console.WriteLine(code);

				await GetTokens();

				//Console.WriteLine("ID Token");
				//Console.WriteLine(idToken);
				//Console.WriteLine("Access Token");
				//Console.WriteLine(accessToken);
			}
			else
			{
				Console.WriteLine("Unable to extract auth code");
			}
		}
		else if(idToken != "")
		{
			//Console.WriteLine("Already logged in");
			//Console.WriteLine(idToken);
		}
		else
		{
			Console.WriteLine("Unable to login");
		}

		// Web UI stuff:
		try
		{
			languages = await getLanguages();
			inputLanguage = languages.FirstOrDefault().Language;
			outputLanguage = languages.FirstOrDefault().Language;

			Documents = await getDocumentNames();
			documentID = Documents.FirstOrDefault().DocumentID;
		}
		catch(Exception ex)
		{
			languages = new List<Languages> {  };
			inputLanguage = languages.FirstOrDefault().Language;
			outputLanguage = languages.FirstOrDefault().Language;

			Documents = new List<Document> { };
			documentID = Documents.FirstOrDefault().DocumentID;
		}
	}

	private static void ReadSecrets()
	{
		try
		{
			using(StreamReader reader = new StreamReader("Components\\Pages\\client_secrets.json"))
			{
				string json = reader.ReadToEnd();
				JsonDocument document = JsonDocument.Parse(json);
				JsonElement root = document.RootElement;
				clientId = root.GetProperty("web").GetProperty("client_id").GetString();
				clientSecret = root.GetProperty("web").GetProperty("client_secret").GetString();
				redirectUri = root.GetProperty("web").GetProperty("redirect_uri").GetString();
			}
			Console.WriteLine("Successfully read in Json");
		}
		catch
		{
			Console.WriteLine("Error Reading in Json");
		}
	}

	private async Task GetTokens()
	{
		var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
		{
			Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["code"] = code,
				["client_id"] = clientId,
				["client_secret"] = clientSecret,
				["redirect_uri"] = redirectUri,
				["grant_type"] = "authorization_code"
			})
		};

		using(var client = new HttpClient())
		{
			var response = await client.SendAsync(tokenRequest);
			if(response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine("Success");
				JsonDocument jsonDoc = JsonDocument.Parse(content);
				if(jsonDoc.RootElement.TryGetProperty("access_token", out JsonElement accessTokenElement) &&
					jsonDoc.RootElement.TryGetProperty("id_token", out JsonElement idTokenElement))
				{
					//Console.WriteLine("Writing tokens");
					//Console.WriteLine(accessTokenElement);
					accessToken = accessTokenElement.GetString();
					idToken = idTokenElement.GetString();
				}
				else
				{
					Console.WriteLine("Failed to parse JSON");
				}
			}
			else
			{
				Console.WriteLine("Failure");
			}
		}
	}

	private async Task<string> GetCode()
	{
		var urlBuilder = new UriBuilder("https://accounts.google.com/o/oauth2/v2/auth");
		var queryParameters = new Dictionary<string, string>()
		{
			["client_id"] = clientId,
			["redirect_uri"] = redirectUri,
			["scope"] = scopes,
			["response_type"] = "code",
			["access_type"] = "offline"
		};

		urlBuilder.Query = string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

		return urlBuilder.ToString();
	}

	// Web UI code:
	public string? fileContent { get; set; }
	public string? newDocumentName;
	public string messageInfo = "Provide valid input";
	public List<Languages> languages = new List<Languages> { };
	public List<Document> Documents = new List<Document> { };
	public string? inputLanguage, outputLanguage;
	public int maxFileSizeBytes = 1024, documentID;

	public async Task CallOnInits()
	{
		this.OnInitialized();
		await OnInitializedAsync();
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

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
		StateHasChanged();
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
		StateHasChanged();
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
				StateHasChanged();
			}
			else
			{
				Console.WriteLine($"Error: {response.StatusCode}\u001b[0m");
			}
		}
		await CallOnInits();
		StateHasChanged();
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
