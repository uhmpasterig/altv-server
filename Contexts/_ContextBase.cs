using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using server.Models;

namespace server.Contexts;

public abstract class ServerContextBase : DbContext
{
  protected virtual void OnModelCreatingPartial(ModelBuilder modelBuilder) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
  }
}