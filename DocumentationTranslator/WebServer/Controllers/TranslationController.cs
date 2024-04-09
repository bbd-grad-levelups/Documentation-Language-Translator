using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DocTranslatorServer.Models;
using DocTranslatorServer.Data;

namespace DocTranslatorServer.Controllers
{
  [Route("/[controller]")]
  [ApiController]
  public class TranslationController(DocumentContext documentContext, UserContext userContext, LanguageContext languageContext, IHttpContextAccessor httpContext) : ControllerBase
  {
    private readonly DocumentContext _docContext = documentContext;
    private readonly UserContext _usrContext = userContext;
    private readonly LanguageContext _lanContext = languageContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContext;

    [HttpGet("/document/names")]
    public async Task<ActionResult<IEnumerable<DocName>>> GetUserDocumentNames()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _docContext.Document.Where(e => e.UserID == userId)
        .Select(document => ConvertDocToDocName(document)).ToListAsync();

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

      string endpoint = "microsoft-translator-text.p.rapidapi.com";
      string apiKey = Environment.GetEnvironmentVariable("DocServer_TranslationAPIKey") ?? throw new KeyNotFoundException("Could not load environment variable: ApiKey");

      var translatedString = await Translator.CallTranslatorAPI("", language.Abbreviation, document.DocumentContent, apiKey, endpoint);

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

        return Ok(ConvertDocToTextDoc(docEntry.Entity, userId));
      }

      return Ok("");
    }

    [HttpGet("/document/{id}")]
    public async Task<ActionResult<TextDocument>> GetSpecificDocument(int id)
    {
      var document = await _docContext.Document.FindAsync(id);
      document.Language = await _lanContext.Language.FindAsync(document.LanguageID);

      var context = _httpContextAccessor.HttpContext;

      if ((document != null || document.Language != null) &&
          context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {

        var fileData = await GetDocumentFromFile(userId, document.DocumentName);

        var actualDoc = new TextDocument()
        {
          LanguageID = document.Language.LanguageID,
          DocumentContent = fileData,
          DocumentTitle = document.DocumentName,
          GenTime = document.GenTime
        };

        return Ok(actualDoc);

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
      string filePath = Path.Combine("DocumentFiles", userID.ToString(), $"{title}.txt");

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

      string filePath = Path.Combine("DocumentFiles", userID.ToString(), $"{title}.txt");
      return await BucketLoader.GetDocumentFromS3Async(bucketName, filePath);

    }

    private async static Task<TextDocument> ConvertDocToTextDoc(Document inputDoc, int userID)
    {
      return new TextDocument()
      {
        DocumentContent = await GetDocumentFromFile(userID, inputDoc.DocumentName),
        DocumentTitle = inputDoc.DocumentName,
        GenTime = inputDoc.GenTime,
        LanguageID = inputDoc.LanguageID,
        DocumentID = inputDoc.DocumentID
      };
    }

    private static DocName ConvertDocToDocName(Document inputDoc)
    {
      return new DocName(inputDoc.DocumentID, Path.GetFileNameWithoutExtension(inputDoc.DocumentName) ?? "", "");
    }
  }
}
