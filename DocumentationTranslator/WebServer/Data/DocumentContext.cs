using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;

namespace DocTranslatorServer.Data;

public class DocumentContext(DbContextOptions<DocumentContext> options) : DbContext(options)
{
  public DbSet<Document> Document { get; set; } = default!;
}
