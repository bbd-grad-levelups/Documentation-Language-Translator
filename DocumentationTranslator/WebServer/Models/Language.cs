using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocTranslatorServer.Models;

[Table("Languages")]
public class Languages
{
  [Key]
  public int LanguageID { get; set; }

  [Required]
  [StringLength(100)]
  public string? Language { get; set; }

  [Required]
  [StringLength(5)]
  public string? Abbreviation { get; set; }
}