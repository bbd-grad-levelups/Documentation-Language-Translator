using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;

namespace DocTranslatorServer.Data;

public class LanguageContext(DbContextOptions<LanguageContext> options) : DbContext(options)
{
  public DbSet<Languages> Language { get; set; } = default!;
}
