namespace DocTranslatorServer.Models;

public record TextDocument
{
  public int LanguageID { get; set; }
  public virtual Language? Language { get; set; }

  public DateTime? GenTime { get; set; }

  public string DocumentContent { get; set; } = string.Empty;

  public string DocumentTitle { get; set; } = string.Empty;

}