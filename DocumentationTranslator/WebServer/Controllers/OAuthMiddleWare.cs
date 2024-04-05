using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocTranslatorServer.Controllers;
public class OAuthMiddleWare : IMiddleware
{
  private readonly IHttpClientFactory _httpClientFactory;

  public OAuthMiddleWare(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    var JWTToken = context.Request.Headers["Authorization"];

    // TODO: OAuth verification.
    Console.WriteLine("Authorisation not implemented! Skipping");
    Console.WriteLine($"Token: {JWTToken}");

    // I want this for use in the next MiddleWare steps.
    // Any kind of unique identifier tbh.
    context.Items["UID"] = "testUID";

    await next(context);
  }
}
