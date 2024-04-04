using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocTranslatorServer.Controllers;


[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

  [HttpGet("test")]
  public async Task<string> TestCall(string translatedString, string language)
  {
    var client = new HttpClient();
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new Uri($"https://microsoft-translator-text.p.rapidapi.com/translate?to%5B0%5D={language}&api-version=3.0&profanityAction=NoAction&textType=plain"),
      Headers =
        {
            { "X-RapidAPI-Key", "d91f0fc765mshf6a55b818ddc0edp1e5181jsn85bae4e96f78" },
            { "X-RapidAPI-Host", "microsoft-translator-text.p.rapidapi.com" },
        },
      Content = new StringContent("[\r\n    {\r\n        \"Text\": \"" + translatedString + "\"\r\n    }\r\n]")
      {
        Headers =
            {
                ContentType = new MediaTypeHeaderValue("application/json")
            }
      }
    };

    using (var response = await client.SendAsync(request))
    {
      response.EnsureSuccessStatusCode();
      var body = await response.Content.ReadAsStringAsync();
      return body; // Return the translated text
    }
  }
}
