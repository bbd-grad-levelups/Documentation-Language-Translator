using System;
using System.Text.Json;

namespace Cli.Commands
{
    internal class LoginCommand
    {
        private static string scopes = "profile email";

        public static async Task<(string idToken, string accessToken, string name, string email)> Run(string id, string secret, string redirectUri)
        {
            var urlBuilder = new UriBuilder("https://accounts.google.com/o/oauth2/v2/auth");
            var queryParameters = new Dictionary<string, string>()
            {
                ["client_id"] = id,
                ["redirect_uri"] = redirectUri,
                ["scope"] = scopes,
                ["response_type"] = "code",
                ["access_type"] = "offline"
            };
            urlBuilder.Query = string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            
            Console.WriteLine("\u001b[33mOpen the following URL in your browser:\u001b[0m");
            Console.WriteLine(urlBuilder.ToString());
            Console.WriteLine("\u001b[33mEnter the authorization code below:\u001b[0m");
            string authorizationCode = Console.ReadLine().Trim();

            string tokenEndpoint = "https://oauth2.googleapis.com/token";
            var tokenRequestContent = new FormUrlEncodedContent(new[]
            {
                        new KeyValuePair<string, string>("code", authorizationCode),
                        new KeyValuePair<string, string>("client_id", id),
                        new KeyValuePair<string, string>("client_secret", secret),
                        new KeyValuePair<string, string>("redirect_uri", redirectUri),
                        new KeyValuePair<string, string>("grant_type", "authorization_code")
                    });

            var client = new HttpClient();
            var tokenResponse = await client.PostAsync(tokenEndpoint, tokenRequestContent);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("\u001b[31mError logging in\u001b[0m");
                return (null, null, null, null);
            }
            else
            {
                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                JsonDocument jsonDoc = JsonDocument.Parse(tokenResponseJson);
                if (jsonDoc.RootElement.TryGetProperty("access_token", out JsonElement accessTokenElement) &&
                    jsonDoc.RootElement.TryGetProperty("id_token", out JsonElement idTokenElement))
                {
                    string accessToken = accessTokenElement.GetString();
                    string idToken = idTokenElement.GetString();

                    // Console.WriteLine("ID token:");
                    // Console.WriteLine($"{idToken}\n\n");

                    string name, email;

                    (name, email) = await getUserInfo(accessToken);

                    Console.WriteLine("\u001b[32mSuccessfully logged in\u001b[0m");
                    return (idToken, accessToken, name, email);
                }
                else
                {
                    Console.WriteLine("\u001b[31mFailed to parse JSON\u001b[0m");
                    return (null, null, null, null);
                }
            }

        }

        public static async Task<(string name, string email)> getUserInfo(string accessToken)
        {
            string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var userInfoResponse = await client.GetAsync(userInfoEndpoint);

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("\u001b[31mError fetching user information\u001b[0m");
                return (null, null);
            }
            else
            {
                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                JsonDocument userInfoDoc = JsonDocument.Parse(userInfoJson);

                if (userInfoDoc.RootElement.TryGetProperty("name", out JsonElement nameElement) &&
                    userInfoDoc.RootElement.TryGetProperty("email", out JsonElement emailElement))
                {
                    string name = nameElement.GetString();
                    string email = emailElement.GetString();

                    return (name, email);
                }
                else
                {
                    Console.WriteLine("\u001b[31mFailed to parse user information JSON\u001b[0m");
                    return (null, null);
                }
            }
        }
    }
}
