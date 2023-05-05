using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using server.Models;

namespace server.Contexts;

public partial class ServerContext : DbContext
{
  public virtual DbSet<Faction> Factions { get; set; }
  public virtual DbSet<Faction_Ranks> Faction_Ranks { get; set; }

  private void CoreContextConfiguring(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Faction>(entity =>
    {
      entity.HasMany(f => f.Ranks)
        .WithOne(r => r.Faction)
        .HasForeignKey(r => r.faction_id)
        .HasPrincipalKey(f => f.id);
    });
  }
}