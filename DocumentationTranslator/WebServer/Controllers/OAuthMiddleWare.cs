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

    // Get the key ID from the JWT header
    string[] parts = JWTToken.Split('.');

    context.Items["UID"] = GetUID(parts[1]);

    await next(context);
  }

  static string GetUID(string payload)
  {
    string payloadJson = DecodeBase64Url(payload);
    JsonDocument jsonDocument = JsonDocument.Parse(payloadJson);
    JsonElement root = jsonDocument.RootElement;
    return root.GetProperty("sub").ToString();
  }

  static string DecodeBase64Url(string base64Url)
  {
    string padded = base64Url.Length % 4 == 0 ? base64Url : string.Concat(base64Url, "====".AsSpan(base64Url.Length % 4));
    string base64 = padded.Replace("_", "/").Replace("-", "+");
    byte[] bytes = Convert.FromBase64String(base64);
    return Encoding.UTF8.GetString(bytes);
  }
}
