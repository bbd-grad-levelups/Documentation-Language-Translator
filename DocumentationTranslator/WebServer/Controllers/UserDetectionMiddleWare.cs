using DocTranslatorServer.Models;

namespace DocTranslatorServer.Controllers;
public class UserDetectionMiddleWare : IMiddleware
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly UserContext _context;

  public UserDetectionMiddleWare(IHttpClientFactory httpClientFactory, UserContext controller)
  {
    _httpClientFactory = httpClientFactory;
    _context = controller;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    
    string userName = context.Items["UserName"] as string ?? "unknown";

    if (!_context.User.Any(e => e.Username == userName)) 
    {
      var newUser = new User() {
        UserID = 0,
        Username = userName
      };
      _context.Add(newUser);
      _context.SaveChanges();
    }

    var user = _context.User.FirstOrDefault(u => u.Username == userName) ?? new User();

    context.Items["userID"] = user.UserID;

    await next(context);
  }
}