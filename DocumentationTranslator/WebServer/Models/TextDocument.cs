namespace DocTranslatorServer.Models;

public class TextDocument
{
  public int DocumentID { get; set; }
  
  public int LanguageID { get; set; }

  public DateTime? GenTime { get; set; }

  public string DocumentTitle { get; set; } = string.Empty;

  public string DocumentContent { get; set; } = string.Empty;

}