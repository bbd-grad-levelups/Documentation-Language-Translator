using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocTranslatorServer.Controllers;
public class GitHubValidation : IMiddleware
{
  private readonly IHttpClientFactory _httpClientFactory;

  public GitHubValidation(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    // GitHub API call
    var httpClient = _httpClientFactory.CreateClient();
    var token = context.Request.Headers["Authorization"];
    httpClient.DefaultRequestHeaders.Add("Authorization", token.ToString());

    var response = await httpClient.GetAsync("https://api.github.com/user");

    Console.WriteLine($"Token: {token}");
    Console.WriteLine($"Things: {response}");


    // if (!response.IsSuccessStatusCode)
    // {
    //   context.Response.StatusCode = 401; // Unauthorized
    //   await context.Response.WriteAsync("Invalid GitHub token");
    //   return;
    // }


    // save the username and UID for later
    context.Items["UserName"] = "testUser";
    context.Items["UID"] = "testUID";







    await next(context);
  }
}
