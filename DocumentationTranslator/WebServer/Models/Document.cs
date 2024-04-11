using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocTranslatorServer.Models;

[Table("Documents")]
public class Document
{
  [Key]
  public int DocumentID { get; set; }

  public int UserID { get; set; }
  [ForeignKey("UserID")]
  public virtual User? User { get; set; }

  public int LanguageID { get; set; }
  [ForeignKey("LanguageID")]
  public virtual Languages? Language { get; set; }

  [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
  public DateTime GenTime { get; set; }

  [Required]
  [StringLength(500)]
  public string? DocumentName { get; set; }
}