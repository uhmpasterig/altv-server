using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models;

public partial class ServerContext : DbContext
{
  public ServerContext() { }
  public ServerContext(DbContextOptions<ServerContext> options) : base(options) { }

  public virtual DbSet<Player> Player { get; set; }
  public virtual DbSet<Vehicle> Vehicle { get; set; }
  public virtual DbSet<Storage> Storages { get; set; }
  public virtual DbSet<Item> Items { get; set; }

  public virtual DbSet<Fraktion> Fraktionen { get; set; }
  public virtual DbSet<Fraktion_rang> Fraktionen_range { get; set; }
  public virtual DbSet<Fraktion_ug> Fraktionen_ugs { get; set; }

  public virtual DbSet<sammler_farming_data> sammler_farming_data { get; set; }
  public virtual DbSet<verarbeiter_farming_data> verarbeiter_farming_data { get; set; }

  public virtual DbSet<Garage> Garage { get; set; }
  public virtual DbSet<GarageSpawn> GarageSpawns { get; set; }

  public virtual DbSet<Shop> Shops { get; set; }
  public virtual DbSet<ShopItems> ShopItems { get; set; }

  public virtual DbSet<Bank> Banks { get; set; }

  public virtual DbSet<Prop> Props { get; set; }

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

    modelBuilder.Entity<Item>(entity =>
    {
      entity.ToTable("item_data");
    });
  
    // FRAKTIONEN
    #region FRAKTION
    modelBuilder.Entity<Fraktion>(entity =>
    {
      entity.ToTable("fraktionen");
    });

    modelBuilder.Entity<Fraktion_rang>(entity =>
    {
      entity.ToTable("fraktions_raenge");
    });

    modelBuilder.Entity<Fraktion_ug>(entity =>
    {
      entity.ToTable("fraktions_ugs");
    });
    #endregion

    // FARMING
    #region FARMING
    modelBuilder.Entity<sammler_farming_data>(entity =>
    {
      entity.ToTable("sammler_farming_data");
    });

    modelBuilder.Entity<verarbeiter_farming_data>(entity =>
    {
      entity.ToTable("verarbeiter_farming_data");
    });
    #endregion

    // GARAGE
    #region GARAGE
    modelBuilder.Entity<Garage>(entity =>
    {
      entity.ToTable("garage");
    });

    modelBuilder.Entity<GarageSpawn>(entity =>
    {
      entity.ToTable("garage_spawns");
    });
    #endregion

    // SHOP
    #region SHOP
    modelBuilder.Entity<Shop>(entity =>
    {
      entity.ToTable("shops");
    });

    modelBuilder.Entity<ShopItems>(entity =>
    {
      entity.ToTable("shop_items");
    });
    #endregion

     modelBuilder.Entity<Prop>(entity =>
    {
      entity.ToTable("custom_props");
    });

    modelBuilder.Entity<Bank>(entity =>
    {
      entity.ToTable("banks");
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}