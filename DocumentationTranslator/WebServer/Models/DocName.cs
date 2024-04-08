namespace DocTranslatorServer.Models;

public class DocName
{
  public int DocumentID { get; set; }

  public string DocumentName { get; set; }

  public DocName(int docID, string docName) 
  {
    DocumentID = docID;
    DocumentName = docName;
  }
}