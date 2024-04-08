using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DocTranslatorServer.Models;
using DocTranslatorServer.Data;

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

    // GET: api/User
    [HttpGet("/user")]
    public async Task<ActionResult<string>> GetUser()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _usrContext.User.FindAsync(userId);
        if (thing != null)
        { 
          return Ok(thing.Username ?? "unknown");
        }
        else
        {
          return NotFound();
        }
        
      }
      else
      {
        return NotFound();
      }
    }


    [HttpGet("/document")]
    public async Task<ActionResult<IEnumerable<TextDocument>>> GetUserDocuments()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var userDocList = await _docContext.Document.Where(e => e.UserID == userId).Select(document => ConvertDocToTextDoc(document)).ToListAsync();

        return Ok(userDocList);
      }
      else
      {
        return NotFound();
      }
    }

    [HttpGet("/document/names")]
    public async Task<ActionResult<IEnumerable<DocName>>> GetUserDocumentNames()
    {
      // Access context's user id value
      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var thing = await _docContext.Document.Where(e => e.UserID == userId).Select(document => ConvertDocToDocName(document)).ToListAsync();

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
      string apiKey = _configuration.GetValue("APIKey", "") ?? "";

      var translatedString = await Translator.CallTranslatorAPI("", language.Abbreviation, document.DocumentContent, apiKey, endpoint);

      var context = _httpContextAccessor.HttpContext;

      if (context != null && context.Items.TryGetValue("userID", out var userIdObj) && userIdObj is int userId)
      {
        var newPath = SaveToFile(userId.ToString(), document.DocumentTitle, translatedString);

        var newDoc = new Document()
        {
          LanguageID = language.LanguageID,
          DocumentName = newPath,
          UserID = userId,
          GenTime = DateTime.Now,
        };
        var docEntry = _docContext.Document.Add(newDoc);
        await _docContext.SaveChangesAsync();

        var newTextDoc = ConvertDocToTextDoc(docEntry.Entity);

        return Ok(newTextDoc);
      }

      return Ok("");
    }

    [HttpGet("/document/{id}")]
    public async Task<ActionResult<TextDocument>> GetSpecificDocument(int id)
    {
      var document = await _docContext.Document.FindAsync(id);
      document.Language = await _lanContext.Language.FindAsync(document.LanguageID);

      if (document == null || document.Language == null)
      {
        return NotFound();
      }
      else
      {
        var fileData = GetDocumentFromFile(document.DocumentName);
        var documentName = Path.GetFileNameWithoutExtension(document.DocumentName);

        var actualDoc = new TextDocument()
        {
          LanguageID = document.Language.LanguageID,
          DocumentContent = fileData,
          DocumentTitle = documentName,
          GenTime = document.GenTime
        };

        return Ok(actualDoc);
      }
    }

    [HttpGet("/languages")]
    public async Task<ActionResult<IEnumerable<Languages>>> GetLanguage()
    {
      return await _lanContext.Language.ToListAsync();
    }

    private static string SaveToFile(string username, string title, string content)
    {
      string filePath = Path.Combine("DocumentFiles", username, $"{title}.txt");

      // Ensure that the directory exists
      string directoryPath = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      // Create or append to the file and write data to it
      using (StreamWriter writer = System.IO.File.AppendText(filePath))
      {
        writer.WriteLine(content);
      }

      System.IO.File.WriteAllText(filePath, content);
      return filePath;
    }

    private static string GetDocumentFromFile(string path)
    {
      try
      {
        // Read the contents of the file
        string content = System.IO.File.ReadAllText(path);
        return content;
      }
      catch (Exception ex)
      {
        // Handle any exceptions (e.g., file not found, permission issues, etc.)
        Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        return null; // Or throw an exception, or return a default value, based on your requirements
      }
    }

    private static TextDocument ConvertDocToTextDoc(Document inputDoc)
    {
      return new TextDocument()
      {
        DocumentContent = GetDocumentFromFile(inputDoc.DocumentName),
        DocumentTitle = Path.GetFileNameWithoutExtension(inputDoc.DocumentName),
        GenTime = inputDoc.GenTime,
        LanguageID = inputDoc.LanguageID,
        DocumentID = inputDoc.DocumentID
      };
    }

    private static DocName ConvertDocToDocName(Document inputDoc)
    {
      return new DocName(inputDoc.DocumentID, Path.GetFileNameWithoutExtension(inputDoc.DocumentName) ?? "");
    }
  }
}
