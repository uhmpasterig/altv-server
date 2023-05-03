using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models;

public partial class PlayerContext : DbContext
{
  public PlayerContext() { }

  public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseMySql("server=45.157.233.24;database=server_player;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
        new MySqlServerVersion(new Version(8, 0, 25)));
    }
  }

  public virtual DbSet<Player> Players { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Player>(entity =>
    {
      entity.HasMany(p => p.Weapons)
        .WithOne(w => w.Player)
        .HasForeignKey(w => w.player_id)
        .HasPrincipalKey(p => p.id);

      entity.HasOne(p => p.WorldOffset)
        .WithOne(w => w.Player)
        .HasForeignKey<Player_WorldOffset>(w => w.player_id)
        .HasPrincipalKey<Player>(w => w.id);

      entity.HasOne(p => p.Identifier)
        .WithOne(i => i.Player)
        .HasForeignKey<Player_Identifier>(i => i.player_id)
        .HasPrincipalKey<Player>(i => i.id);

      entity.HasOne(p => p.Vitals)
        .WithOne(v => v.Player)
        .HasForeignKey<Player_Vitals>(v => v.player_id)
        .HasPrincipalKey<Player>(v => v.id);

      entity.HasOne(p => p.Accounts)
        .WithOne(a => a.Player)
        .HasForeignKey<Player_Accounts>(a => a.player_id)
        .HasPrincipalKey<Player>(a => a.id);
    });

    OnModelCreatingPartial(modelBuilder);
  }

  // function to create a new instance of the PlayerContext class
  public PlayerContext Instance => new PlayerContext();

  // function to clear the instance and dispose of the PlayerContext class instance after saving changes
  public async Task ClearInstance()
  {
    await this.SaveChangesAsync();
    await this.DisposeAsync();
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}