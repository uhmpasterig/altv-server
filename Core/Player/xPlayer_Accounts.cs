using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;
using AltV.Net.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace server.Core;

public partial class xPlayer
{
  public Player_Accounts Accounts { get; set; }

  private async Task _loadAccounts(Player_Accounts Accounts)
  {
    if (Accounts == null)
    {
      this.Accounts = new Player_Accounts().Default;
      return;
    }

    this.Accounts = Accounts;
  }

  public async Task SaveAccounts()
  {
    var ctx = new PlayerContext().Instance;
    var dbPlayer = await ctx.Players
      .Include(p => p.Accounts)
      .Where(p => p.id == this.id)
      .FirstOrDefaultAsync();

    if (dbPlayer == null)
    {
      Console.WriteLine($"Player {this.name} not found in database");
      return;
    }

    dbPlayer.Accounts = this.Accounts;
    await ctx.SaveChangesAsync();
    await ctx.ClearInstance();
  }
}