using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;

namespace DataAccess
{
  public partial class SqliteBloggingContext : BloggingContext
  {
    private readonly string? _connectionString;

    public SqliteBloggingContext(IConfiguration config) {
      _connectionString = config.GetConnectionString("SqliteConnectionString");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}