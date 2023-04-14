using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models;

public partial class ServerContext : DbContext
{
  public ServerContext() {}
  public ServerContext(DbContextOptions<ServerContext> options) : base(options) { }

  public virtual DbSet<Player> Player { get; set; }
  public virtual DbSet<Vehicle> Vehicle { get; set; }
  public virtual DbSet<Storage> Storages { get; set; }
  public virtual DbSet<Models.Item> Items { get; set; }
  public virtual DbSet<Models.BadFrak> BadFrak { get; set; }
  public virtual DbSet<Models.sammler_farming_data> sammler_farming_data { get; set; }
  public virtual DbSet<Models.sammler_verarbeiter_data> sammler_verarbeiter_data { get; set; }


  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseMySql("server=45.157.233.24;database=server;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
        new MySqlServerVersion(new Version(8, 0, 25)));
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Player>(entity =>
    {
      entity.ToTable("player");

      entity.Property(e => e.permaId)
          .HasColumnName("id");
    });

    modelBuilder.Entity<Vehicle>(entity =>
    {
      entity.ToTable("vehicle");
    });

    modelBuilder.Entity<Storage>(entity =>
    {
      entity.ToTable("storages");
    });

    modelBuilder.Entity<Models.Item>(entity =>
    {
      entity.ToTable("item_data");
    });

    modelBuilder.Entity<Models.BadFrak>(entity =>
    {
      entity.ToTable("fraktionen");
    });

    modelBuilder.Entity<Models.sammler_farming_data>(entity =>
    {
      entity.HasNoKey();
      entity.ToTable("sammler_farming_data");
    });
    modelBuilder.Entity<Models.sammler_verarbeiter_data>(entity =>
    {
      entity.HasNoKey();
      entity.ToTable("sammler_verarbeiter_data");
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}