using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;

namespace DataAccess
{
  public partial class SqlBloggingContext : BloggingContext
  {
    private readonly string _connectionString;

    public SqlBloggingContext(IConfiguration config) {
      _connectionString = config.GetConnectionString("SqlServerConnectionString");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}