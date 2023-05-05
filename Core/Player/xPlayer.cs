using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using server.Models;
using server.Contexts;

namespace server.Core;

public partial class xPlayer : AsyncPlayer
{
  public int id { get; set; }
  public string name { get; set; }

  public xPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id) { }

  public async Task Respawn(float x = 0, float y = 0, float z = 0)
  {
    this.Spawn(new Position(x, y, z), 0);
  }

  public async Task Load(Models.Player _player)
  {
    this.id = _player.id;
    this.name = _player.name;
    this.Model = _player.ped;

    // Load the players's variables from the database parallel to speed up the process
    var _task = new List<Task>
    {
      this._loadWeapons(_player.Weapons),
      this._loadAccounts(_player.Accounts),
      this._loadVitals(_player.Vitals)
    };
    await Task.WhenAll(_task);

    // Spawn the player and set it frozen
    await this.Respawn();

    // Set the player's position and rotation to the one stored in the database
    await this._loadWorldOffset(_player.WorldOffset);

    this.Frozen = false;
  }

  public async Task Save()
  {
    var ctx = ServerContext.Instance;
    var dbPlayer = await ctx.Players
      .Include(p => p.Weapons)
      .Include(p => p.Accounts)
      .Include(p => p.Vitals)
      .Include(p => p.WorldOffset)
      .Where(p => p.id == this.id)
      .FirstOrDefaultAsync();

    if (dbPlayer == null) return;
    dbPlayer.Weapons = this.Weapons;
    dbPlayer.Accounts = this.Accounts;

    var _task = new List<Task>
    {
      dbPlayer.Vitals.SaveAsync(this.Vitals),
      dbPlayer.WorldOffset.SaveAsync(this.WorldOffset)
    };

    await Task.WhenAll(_task);

    await ctx.SaveChangesAsync();
    await ctx.ClearInstance();
  }
}