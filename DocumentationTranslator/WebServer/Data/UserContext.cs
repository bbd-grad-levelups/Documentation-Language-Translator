using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
  public UserContext(DbContextOptions<UserContext> options)
    : base(options)
  {
  }

  public DbSet<DocTranslatorServer.Models.User> User { get; set; } = default!;
}
