using System.ComponentModel.DataAnnotations;

namespace DocTranslatorServer.Models;

public class Language
{
  [Key]
  public int LanguageID { get; set; }

  [Required]
  [StringLength(100)]
  public string? LanguageName { get; set; }

  [Required]
  [StringLength(5)]
  public string? Abbreviation { get; set; }

  public virtual ICollection<Document>? Documents { get; set; }
}