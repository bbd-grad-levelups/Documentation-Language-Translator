using Microsoft.EntityFrameworkCore;

public class DocumentContext : DbContext
{
  public DocumentContext(DbContextOptions<DocumentContext> options)
    : base(options)
  {
  }

  public DbSet<DocTranslatorServer.Models.Document> Document { get; set; } = default!;
}
