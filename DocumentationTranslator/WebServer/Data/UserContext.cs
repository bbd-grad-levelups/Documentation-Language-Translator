using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;

namespace DocTranslatorServer.Data;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
  public DbSet<User> User { get; set; } = default!;
}
