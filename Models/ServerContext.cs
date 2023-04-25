using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models;

public partial class ServerContext : DbContext
{
  public ServerContext() { }
  public ServerContext(DbContextOptions<ServerContext> options) : base(options) { }

  public virtual DbSet<Player> Players { get; set; }
  public virtual DbSet<Vehicle> Vehicles { get; set; }
  public virtual DbSet<Storage> Storages { get; set; }
  public virtual DbSet<Item> Items { get; set; }

  public virtual DbSet<Faction> Factions { get; set; }
  public virtual DbSet<Faction_rank> Faction_ranks { get; set; }
  public virtual DbSet<Faction_ug> Faction_ugs { get; set; }

  public virtual DbSet<Farming_Collector> Farming_Collectors { get; set; }
  public virtual DbSet<Farming_Processor> Farming_Processors { get; set; }

  public virtual DbSet<Garage> Garages { get; set; }
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
    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}