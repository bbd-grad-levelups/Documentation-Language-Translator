using Microsoft.EntityFrameworkCore;

namespace DocTranslatorServer.Models;

public class DocumentsContext : DbContext
{
  public DocumentsContext(DbContextOptions<DocumentsContext> options)
    : base(options)
  {

  }

  public DbSet<Documents> DocumentsItems { get; set; } = null!;
}