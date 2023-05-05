using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using server.Models;

namespace server.Contexts;

public partial class ServerContext : DbContext, IDisposable
{
  public ServerContext() { }

  public ServerContext(DbContextOptions<ServerContext> options) : base(options) { }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseMySql("server=45.157.233.24;database=altserver;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
        new MySqlServerVersion(new Version(8, 0, 25)));
    }
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    VehicleContextConfiguring(modelBuilder);
    CoreContextConfiguring(modelBuilder);
    PlayerContextConfiguring(modelBuilder);
    base.OnModelCreating(modelBuilder);
  }

  // function to create a new instance of the ServerContext class
  public static ServerContext Instance => new ServerContext();

  // function to clear the instance and dispose of the ServerContext class instance after saving changes
  public async Task ClearInstance()
  {
    await this.SaveChangesAsync();
    await this.DisposeAsync();
  }
}