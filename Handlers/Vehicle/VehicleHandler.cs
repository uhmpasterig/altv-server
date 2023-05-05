using AltV.Net.Enums;
using server.Core;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Events;
using server.Contexts;
using server.Extensions;
using AltV.Net.Async;

using server.Handlers.Logger;

namespace server.Handlers.Vehicle;

public partial class VehicleHandler : IVehicleHandler, ILoadEvent
{
  public Dictionary<int, xVehicle> Vehicles = new Dictionary<int, xVehicle>();

  ILogger _logger;
  public VehicleHandler(ILogger logger)
  {
    _logger = logger;
  }

  public async Task<xVehicle> CreateVehicle(Models.Vehicle vehicle)
  {
    xVehicle veh = (xVehicle)await AltAsync.CreateVehicle(vehicle.model, vehicle.WorldOffset.Position, vehicle.WorldOffset.Rotation);
    _logger.Log($"Created vehicle {veh.id} at {veh.Position.ToString()}");
    await veh.Load(vehicle);
    return veh;
  }

  public async void LoadVehiclesFromDatabase()
  {
    var ctx = ServerContext.Instance;

    _logger.Log("Loading vehicles from database");
    _logger.Error(ctx.Vehicles.Count().ToString());
    ctx.Vehicles.Include(v => v.WorldOffset).ToList().ForEach(async (veh) =>
    {
      _logger.Log($"Loading vehicle {veh.id} from database");
      xVehicle vehicle = await CreateVehicle(veh);
      Vehicles.Add(vehicle.id, vehicle);
    });

    await ctx.ClearInstance();
  }

  public async Task OnLoad()
  {
    var t = Task.Run(() => LoadVehiclesFromDatabase());
    await t;
  }
}
