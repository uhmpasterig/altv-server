using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using server.Models;

namespace server.Contexts;

public partial class ServerContext : DbContext
{
  public virtual DbSet<Player> Players { get; set; }

  private void PlayerContextConfiguring(ModelBuilder modelBuilder)
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
  }
}