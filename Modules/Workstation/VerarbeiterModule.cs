using server.Core;
using server.Events;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using server.Util.Farming;
using server.Handlers.Vehicle;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

namespace server.Modules.Workstation;

internal class ProcessData
{
}

public class VerarbeiterModule : ILoadEvent
{
  public VerarbeiterModule()
  {
  }
  
  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();
  internal static IStorageHandler _storageHandler = new StorageHandler();

  public async void OnLoad()
  {
  }
}