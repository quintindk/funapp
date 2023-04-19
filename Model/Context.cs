using System.Data.Entity;
using Model;

namespace Context;
public class BloggingContext : DbContext
{
    public DbSet<Blog>? Blogs { get; set; }

    public DbSet<Post>? Posts { get; set; }
}