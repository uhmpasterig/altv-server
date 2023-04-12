using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models;

public partial class ServerContext : DbContext
{
  public ServerContext() { 
    Player = null!;
    Vehicle = null!;
    Storages = null!;
    Items = null!;
    BadFrak = null!;
  }
  public ServerContext(DbContextOptions<ServerContext> options) : base(options) { 
    Player = null!;
    Vehicle = null!;
    Storages = null!;
    Items = null!;
    BadFrak = null!;
  }

  public virtual DbSet<Player> Player { get; set; }
  public virtual DbSet<Vehicle> Vehicle { get; set; }
  public virtual DbSet<Storage> Storages { get; set; }
  public virtual DbSet<Models.Item> Items { get; set; }
  public virtual DbSet<Models.BadFrak> BadFrak { get; set; }


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

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}