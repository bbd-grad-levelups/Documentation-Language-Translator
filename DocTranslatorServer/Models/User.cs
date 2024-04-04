using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocTranslatorServer.Models;

public class User
{
    [Key]
    public int UserID { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    public virtual ICollection<Document> Documents { get; set; }
}