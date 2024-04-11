using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DocTranslatorServer.Models;
using DocTranslatorServer.Data;
using System.Text.Json;

namespace DocTranslatorServer.Controllers
{
  [Route("/[controller]")]
  [ApiController]
  public class TranslationController(DocumentContext documentContext, LanguageContext languageContext, IHttpContextAccessor httpContext) : ControllerBase
  {
    private readonly DocumentContext _docContext = documentContext;
    private readonly LanguageContext _lanContext = languageContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContext;

    [HttpPost("/document")]
    public async Task<ActionResult<TextDocument>> TranslateDocument(TextDocument document)
    {
      document.DocumentContent = document.DocumentContent.Replace("\n","\\n");
      
      // Get language
      var language = await _lanContext.Language.FindAsync(document.LanguageID);
      if (language == null)
        return NotFound("Requested Language Not Found");

      var languageAbbreviation = language.Abbreviation ?? "en";

      string endpoint = "microsoft-translator-text.p.rapidapi.com";
      string apiKey = Environment.GetEnvironmentVariable("DocServer_TranslationAPIKey") ?? throw new KeyNotFoundException("Could not load environment variable: ApiKey");

      var translatedString = await Translator.CallTranslatorAPI("", languageAbbreviation, document.DocumentContent, apiKey, endpoint);

      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        await SaveToFile(userId, document.DocumentTitle, translatedString);

        var newDoc = new Document()
        {
          LanguageID = language.LanguageID,
          DocumentName = document.DocumentTitle,
          UserID = userId,
          GenTime = DateTime.Now,
        };
        var docEntry = _docContext.Document.Add(newDoc);
        await _docContext.SaveChangesAsync();

        var newTextDoc =  new TextDocument()
        {
          DocumentContent = translatedString,
          DocumentTitle = document.DocumentTitle,
          GenTime = docEntry.Entity.GenTime,
          LanguageID = newDoc.LanguageID,
          DocumentID = newDoc.DocumentID
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

      var userIdObj = _httpContextAccessor.HttpContext?.Items["userID"];
      if (userIdObj is not int userId)
        return BadRequest();

      if (document.UserID != userId)
        return NotFound();

      document.Language = await _lanContext.Language.FindAsync(document.LanguageID);

      if (document.Language == null)
        return BadRequest();

      string documentTitle = document.DocumentName ?? "Unknown";

      var fileData = await GetDocumentFromFile(userId, documentTitle);

      var actualDoc = new TextDocument()
      {
        DocumentID = document.DocumentID,
        LanguageID = document.Language.LanguageID,
        DocumentContent = fileData,
        DocumentTitle = documentTitle,
        GenTime = document.GenTime
      };

      return Ok(actualDoc);
    }

    [HttpGet("/document/names")]
    public async Task<ActionResult<IEnumerable<DocName>>> GetUserDocumentNames()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _docContext.Document.Where(e => e.UserID == userId).ToListAsync();

        if (thing.Count > 0)
        {
          List<DocName> docList = [];
          foreach (var item in thing)
          {
            var newTextDoc = await ConvertDocToDocName(item);
            docList.Add(newTextDoc);
          }
          return Ok(docList);
        }
        return Ok(thing);
      }
      else
      {
        return NotFound();
      }
    }

    [HttpGet("/languages")]
    public async Task<ActionResult<IEnumerable<Languages>>> GetLanguage()
    {
      return await _lanContext.Language.ToListAsync();
    }

    private static async Task<bool> SaveToFile(int userID, string title, string content)
    {
      string bucketName;
      try
      {
        bucketName = Environment.GetEnvironmentVariable("DocServer_FileBucket") ?? throw new KeyNotFoundException();
      }
      catch (KeyNotFoundException)
      {
        Console.WriteLine("Failed to load file bucket env variable");
        return false;
      }

      string filePath = $"DocumentFiles/{userID}/{title}";

      return await BucketLoader.PostDocumentToS3Async(bucketName, filePath, content);
    }

    private static async Task<string> GetDocumentFromFile(int userID, string title)
    {
      string bucketName;
      try
      {
        bucketName = Environment.GetEnvironmentVariable("DocServer_FileBucket") ?? throw new KeyNotFoundException();
      }
      catch (KeyNotFoundException)
      {
        Console.WriteLine("Failed to load file bucket env variable");
        return "";
      }

      string filePath = $"DocumentFiles/{userID}/{title}";
      return await BucketLoader.GetDocumentFromS3Async(bucketName, filePath);

    }

    private async Task<DocName> ConvertDocToDocName(Document inputDoc)
    {
      var language = await _lanContext.Language.FindAsync(inputDoc.LanguageID);
      
      return new DocName(inputDoc.DocumentID, inputDoc.DocumentName, language?.Language ?? "");
    }
  }
}
