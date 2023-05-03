using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using Newtonsoft.Json;

using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using server.Models;

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

  public async Task LoadPlayer(Models.Player _player)
  {
    this.Frozen = true;
    this.id = _player.id;
    this.name = _player.name;
    this.Model = _player.ped;

    // Load the player's weapons
    await this._loadWeapons(_player.Weapons);

    // Load the player's vitals
    await this._loadVitals(_player.Vitals);

    // Spawn the player and set it frozen
    await this.Respawn();

    // Set the player's position and rotation to the one stored in the database
    await this._loadWorldOffset(_player.WorldOffset);

    this.Frozen = false;
  }
}