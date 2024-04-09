using System.Net.Http.Headers;
using System.Text.Json;

namespace DocTranslatorServer.Data;

public class Translator
{

  public static async Task<string> CallTranslatorAPI(string originalLanguage, string outputLanguage, string text, string apiKey, string endpoint)
  {
    var client = new HttpClient();
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new Uri($"https://{endpoint}/translate?to%5B0%5D={outputLanguage}&api-version=3.0&profanityAction=NoAction&textType=plain"),
      Headers =
        {
          { "X-RapidAPI-Key", apiKey },
          { "X-RapidAPI-Host", endpoint },
        },
      Content = new StringContent($"[{{\"Text\": \"{text}\"    }}]")
      {
        Headers =
          {
          ContentType = new MediaTypeHeaderValue("application/json")
          }
      }
    };

    using var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();
    Console.WriteLine(body);
    var jsonThing = JsonDocument.Parse(body);
    Console.WriteLine(jsonThing);
    var root = jsonThing.RootElement;
    Console.WriteLine(root);

    var translations = root[0].GetProperty("translations");
    Console.WriteLine(translations);
    var translatedText = translations[0].GetProperty("text").GetString();
    Console.WriteLine(translatedText);
    return  translatedText ?? "";
  }
}