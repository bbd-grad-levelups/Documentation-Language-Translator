﻿using System.Text.Json;

namespace WebApp.Components.Pages;

public partial class Home
{
	
	private static string scopes = "profile email";
	private static string redirectUri = "https://localhost:7138/";
	private static string clientId = "";
	private static string clientSecret = "";
	private string code = "";
	private static string accessToken = "";
	private static string idToken = "";
	private static string fileContent = "qwerty";

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
}
