using Microsoft.EntityFrameworkCore;

public class LanguageContext : DbContext
{
  public LanguageContext(DbContextOptions<LanguageContext> options)
    : base(options)
  {
  }

  public DbSet<DocTranslatorServer.Models.Languages> Language { get; set; } = default!;
}
