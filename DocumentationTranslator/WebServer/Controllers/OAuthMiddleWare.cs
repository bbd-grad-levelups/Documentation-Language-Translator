using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace DocTranslatorServer.Controllers;
public class OAuthMiddleWare(IHttpClientFactory httpClientFactory) : IMiddleware
{
  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {

    var JWTToken = context.Request.Headers.Authorization.ToString();
    if (string.IsNullOrEmpty(JWTToken))
    {
      return;
    }

    string[] parts = JWTToken.Split('.');

    string googlePublicKeysUrl = "https://www.googleapis.com/oauth2/v3/certs";
    string publicKeysJson = await _httpClientFactory.CreateClient().GetStringAsync(googlePublicKeysUrl);

    bool validation = ValidateToken(publicKeysJson, JWTToken);
    if (!validation)
    {
      return;
    }

    context.Items["UID"] = GetUID(parts[1]);

    await next(context);
  }

  private static bool ValidateToken(string publicKeysJson, string token)
  {
    string cli_audience;
    string web_audience;
    try
    {
      cli_audience = Environment.GetEnvironmentVariable("DocServer_CLI_audience") ?? throw new KeyNotFoundException();
      web_audience = Environment.GetEnvironmentVariable("DocServer_WEB_audience") ?? throw new KeyNotFoundException();
    }
    catch (KeyNotFoundException)
    {
      Console.WriteLine("Failed to load audience env variables");
      return false;
    }

    // WEB
    try
    {
      var handler = new JwtSecurityTokenHandler();
      handler.ValidateToken(token, new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKeys = new JsonWebKeySet(publicKeysJson).GetSigningKeys(),
        ValidIssuer = "https://accounts.google.com",
        ValidAudience = web_audience
      }, out var validatedToken);
      return true;
    }
    catch (Exception) { }

    // CLI
    try
    {
      var handler = new JwtSecurityTokenHandler();
      handler.ValidateToken(token, new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKeys = new JsonWebKeySet(publicKeysJson).GetSigningKeys(),
        ValidIssuer = "https://accounts.google.com",
        ValidAudience = cli_audience
      }, out var validatedToken);
      return true;
    }
    catch (Exception) { }

    return false;
  }

  static string GetUID(string base64UrlPayload)
  {
    string padded = base64UrlPayload.Length % 4 == 0 ? base64UrlPayload : string.Concat(base64UrlPayload, "====".AsSpan(base64UrlPayload.Length % 4));
    string base64 = padded.Replace("_", "/").Replace("-", "+");
    byte[] bytes = Convert.FromBase64String(base64);
    string payloadJson = Encoding.UTF8.GetString(bytes);

    JsonDocument jsonDocument = JsonDocument.Parse(payloadJson);
    JsonElement root = jsonDocument.RootElement;
    return root.GetProperty("sub").ToString();
  }
}
