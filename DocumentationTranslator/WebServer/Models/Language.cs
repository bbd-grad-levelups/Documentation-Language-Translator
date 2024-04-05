using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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