using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocTranslatorServer.Models;

[Table("Users")]
public class User
{
  [Key]
  public int UserID { get; set; }

  [Required]
  [StringLength(100)]
  public string? Username { get; set; }
}