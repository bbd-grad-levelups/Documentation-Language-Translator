using System.ComponentModel.DataAnnotations;

namespace DocTranslatorServer.Models;

public class User
{
  [Key]
  public int UserID { get; set; }

  [Required]
  [StringLength(100)]
  public string? Username { get; set; }

  public virtual ICollection<Document>? Documents { get; set; }
}