using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using server.Models;
using server.Contexts;

namespace server.Core;

public enum OWNER_TYPES : int
{
  PLAYER = 0,
  FACTION = 1,
  BUSINESS = 2
}

public partial class xVehicle : AsyncVehicle
{
  public int id { get; set; }
  public string model { get; set; }
  public OWNER_TYPES owner_type { get; set; }
  public int owner_id { get; set; }

  public xVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id) { }

  public async Task Load(Vehicle vehicle)
  {
    this.id = vehicle.id;
    this.model = vehicle.model;
    this.owner_type = (OWNER_TYPES)vehicle.owner_type;
    this.owner_id = vehicle.owner_id;

    // Set the vehicles's position and rotation to the one stored in the database
    await this._loadWorldOffset(vehicle.WorldOffset);
  }
}