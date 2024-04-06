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
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TranslationController(DocumentContext documentContext, UserContext userContext, LanguageContext languageContext, IConfiguration config, IHttpContextAccessor httpContext)
    {
      _docContext = documentContext;
      _usrContext = userContext;
      _lanContext = languageContext;
      _configuration = config;
      _httpContextAccessor = httpContext;
    }

    [HttpPut("/translatetest")]
    public async Task<string> GetTranslation(string originalLanguage, string outputLanguage, string text)
    {
      var translatedString = await CallTranslatorAPI(originalLanguage, outputLanguage, text);
      return translatedString; // Return the translated text
    }

    [HttpGet("/document")]
    public async Task<ActionResult<IEnumerable<Document>>> GetUserDocuments()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _docContext.Document.Where(e => e.UserID == userId).ToListAsync();

        return Ok(thing);
      }
      else
      {
        return NotFound();
      }
    }

    [HttpPost("/document")]
    public async Task<ActionResult<TextDocument>> TranslateDocument(TextDocument document)
    {
      // Get language
      var language = await _lanContext.Language.FindAsync(document.LanguageID);

      var translatedString = await CallTranslatorAPI("", language.Abbreviation, document.DocumentContent);

      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var newDoc = new Document() {
          LanguageID = language.LanguageID,
          Path = translatedString,
          UserID = userId
        };
                var docEntry =    _docContext.Document.Add(newDoc);
              await _docContext.SaveChangesAsync();

        var newTextDoc = new TextDocument() {
          DocumentContent = translatedString,
          DocumentTitle = translatedString,
          GenTime = docEntry.Entity.GenTime,
          LanguageID = docEntry.Entity.LanguageID,
          Language = docEntry.Entity.Language
        };
            return Ok(newTextDoc);

      }

      return Ok("");
    }

    [HttpGet("/document/{id}")]
    public async Task<ActionResult<TextDocument>> GetSpecificDocument(int id)
    {
      var document = await _docContext.Document.FindAsync(id);

      if (document == null)
      {
        return NotFound();
      }
      else
      {
        var actualDoc = new TextDocument()
        {
          LanguageID = document.Language.LanguageID,
          Language = document.Language,
          DocumentContent = document.Path,
          DocumentTitle = document.Path,
          GenTime = document.GenTime
        };

        return Ok(actualDoc);

      }
    }

    [HttpGet("/languages")]
    public async Task<ActionResult<IEnumerable<Language>>> GetLanguage()
    {
      return await _lanContext.Language.ToListAsync();
    }

    [HttpGet("/documents/names")]
    public async Task<ActionResult<IEnumerable<string>>> GetDocumentTitles()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _docContext.Document.Where(e => e.UserID == userId).Select(e => e.Path).ToListAsync();

        return Ok(thing);
      }
      else
      {
        return NotFound();
      }
    }

    private async Task<string> CallTranslatorAPI(string originalLanguage, string outputLanguage, string text)
    {
      string endpoint = "microsoft-translator-text.p.rapidapi.com";
      string apiKey = _configuration.GetValue("APIKey", "") ?? "";
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
      return body;
    }
  }
}
