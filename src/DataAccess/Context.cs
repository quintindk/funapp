using Microsoft.EntityFrameworkCore;
using Model;

namespace DataAccess
{
  public abstract class BloggingContext : DbContext
  {
    public virtual DbSet<Blog>? Blogs { get; set; }

    public virtual DbSet<Post>? Posts { get; set; }
  }
}