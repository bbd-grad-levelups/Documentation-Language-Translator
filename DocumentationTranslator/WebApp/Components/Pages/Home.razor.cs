using System.Text.Json;

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
	public string? documentName, newDocumentName;
	public string messageInfo;
	public List<string> languages;
	public List<string> documentNames;
	public string? inputLanguage, outputLanguage;
	public int maxFileSizeBytes = 1024;

	public void CallOnInit()
	{
		this.OnInitialized();
	}

	protected override void OnInitialized()
	{
		messageInfo = "Provide valid input";
		try
		{
			languages = getLanguages();
			inputLanguage = languages.FirstOrDefault();
			outputLanguage = languages.FirstOrDefault();

			documentNames = getDocumentNames();
			documentName = documentNames.FirstOrDefault();
		}
		catch(Exception ex)
		{
			languages = new List<string> { "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four" };
			inputLanguage = languages.FirstOrDefault();
			outputLanguage = languages.FirstOrDefault();

			documentNames = new List<string> { "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5" };
			documentName = documentNames.FirstOrDefault();
		}

		base.OnInitialized();
	}

	// Functions to call the API:
	private List<string> getLanguages()
	{
		var result = new List<string> { "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four", "one", "two", "three", "four" };
		return result;
	}

	private List<string> getDocumentNames()
	{
		var result = new List<string> { "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5", "doc1", "doc2", "doc3", "doc4", "doc5" };
		return result;
	}

	private void UploadDocument()
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
			// Upload the file:
			var body = $"documentName:{newDocumentName},\ndocumentContents:{fileContent},\ninputLanguage:{inputLanguage},\noutputLanguage:{outputLanguage}";
			// Send it here
			messageInfo = body;
			showPopup = true;
		}
	}

	private void ViewFile()
	{
		messageInfo = $"viewing: {documentName} content";
		showPopup = true;
		// Get docContent
		fileContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
	}
}
