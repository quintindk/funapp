using Microsoft.EntityFrameworkCore;
using Model;

namespace DataAccess
{
  public abstract class BloggingContext : DbContext
  {
    public DbSet<Blog>? Blogs { get; set; }

    public DbSet<Post>? Posts { get; set; }
  }
}