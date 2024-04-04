namespace frontend_cli.Commands
{
    internal class LogoutCommand
    {
        public static async Task Run(string accessToken) 
        {
            string revokeTokenEndpoint = "https://oauth2.googleapis.com/revoke";
            var revokeTokenContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", accessToken)
            });

            var client = new HttpClient();
            var revokeResponse = await client.PostAsync(revokeTokenEndpoint, revokeTokenContent);

            if (revokeResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("\u001b[32mUser logged out successfully.\u001b[0m");
            }
            else
            {
                Console.WriteLine("\u001b[31mError logging out.\u001b[0m");
            }
        }
    }
}
