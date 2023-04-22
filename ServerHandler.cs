using System;
using AltV.Net;
using AltV.Net.Async;
using server.Helpers;
using AltV.Net.Elements.Entities;
using server.Core;
using Autofac;
using Microsoft.Extensions.Logging;
using server.Models;
using AltV.Net.Async.Elements.Entities;

using server.Handlers.Storage;
using server.Handlers.Player;
using server.Handlers.Vehicle;
using server.Handlers.Event;
using server.Handlers.Timer;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace server
{
  public class Resource : AsyncResource
  {
    private IServer _server;

    public override async void OnStart()
    {
      Console.WindowHeight = 1000;
      Console.WindowWidth = 1000;
      Console.BufferHeight = 1000;
      Console.BufferWidth = 1000;
      Console.SetWindowSize(1000, 1000);
      Console.LargestWindowHeight = 1000;
      Console.LargestWindowWidth = 1000;

      using var startup = new Startup();
      startup.Register();

      // context cannot be null
      await using var serverContext = new ServerContext();
      var playerHandler = startup.GetContainer().Resolve<IPlayerHandler>();
      var vehicleHandler = startup.GetContainer().Resolve<IVehicleHandler>();
      var eventHandler = startup.GetContainer().Resolve<IEventHandler>();
      var timerHandler = startup.GetContainer().Resolve<ITimerHandler>();
      var storageHandler = startup.GetContainer().Resolve<IStorageHandler>();


      _server = new Server(serverContext, vehicleHandler, playerHandler, eventHandler, timerHandler, storageHandler);
      _server.Start();
    }
    public override async void OnStop()
    {
      await _server.Stop();
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory() => new xPlayerFactory();
    public override IEntityFactory<IVehicle> GetVehicleFactory() => new xVehicleFactory();
  }
}


public class xPlayerFactory : IEntityFactory<IPlayer>
{
  public IPlayer Create(ICore core, IntPtr playerPointer, ushort id)
  {
    return new xPlayer(core, playerPointer, id);
  }

  public AsyncPlayer CreateAsync(ICore core, IntPtr playerPointer, ushort id)
  {
    return new xPlayer(core, playerPointer, id);
  }
}

public class xVehicleFactory : IEntityFactory<IVehicle>
{
  public IVehicle Create(ICore core, IntPtr vehiclePointer, ushort id)
  {
    return new xVehicle(core, vehiclePointer, id);
  }

  public AsyncVehicle CreateAsync(ICore core, IntPtr vehiclePointer, ushort id)
  {
    return new xVehicle(core, vehiclePointer, id);
  }
}