using Microsoft.EntityFrameworkCore;

public class LanguageContext : DbContext
{
  public LanguageContext(DbContextOptions<LanguageContext> options)
    : base(options)
  {
  }

  public DbSet<DocTranslatorServer.Models.Language> Language { get; set; } = default!;
}
