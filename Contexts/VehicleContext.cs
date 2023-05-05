using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using server.Models;

namespace server.Contexts;

public partial class ServerContext : DbContext
{
  public virtual DbSet<Vehicle> Vehicles { get; set; }

  private void VehicleContextConfiguring(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Vehicle>(entity =>
    {
      entity.HasOne(v => v.WorldOffset)
        .WithOne(w => w.Vehicle)
        .HasForeignKey<Vehicle_WorldOffset>(w => w.vehicle_id)
        .HasPrincipalKey<Vehicle>(v => v.id);
    });
  }
}