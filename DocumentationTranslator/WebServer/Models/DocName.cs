namespace DocTranslatorServer.Models;

public class DocName(int docID, string docName, string language)
{
  public int DocumentID { get; set; } = docID;

  public string DocumentName { get; set; } = docName;

  public string Language { get; set; } = language;
}