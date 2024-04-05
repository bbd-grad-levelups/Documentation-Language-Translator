using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;
using System.Net.Http.Headers;

namespace DocTranslatorServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TranslationController : ControllerBase
  {
    private readonly DocumentContext _docContext;
    private readonly UserContext _usrContext;
    private readonly LanguageContext _lanContext;

    public TranslationController(DocumentContext documentContext, UserContext userContext, LanguageContext languageContext)
    {
      _docContext = documentContext;
      _usrContext = userContext;
      _lanContext = languageContext;
    }

    [HttpPut("/basic")]
    public async Task<string> GetTranslation(string inputLanguage, string text)
    {
      string endpoint = "microsoft-translator-text.p.rapidapi.com";
      var client = new HttpClient();
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"https://{endpoint}/translate?to%5B0%5D={inputLanguage}&api-version=3.0&profanityAction=NoAction&textType=plain"),
        Headers =
        {
          { "X-RapidAPI-Key", "d91f0fc765mshf6a55b818ddc0edp1e5181jsn85bae4e96f78" },
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
      return body; // Return the translated text
    }

  }
}
