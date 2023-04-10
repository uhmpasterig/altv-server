using server.Core;
using server.Models;
using AltV.Net.Data;
using AltV.Net.Enums;

namespace server.Handlers.Vehicle;

public interface IVehicleHandler
{

  Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle);

  Task SaveAllVehicles();
  xVehicle GetClosestVehicle(Position position, int range = 2);
  bool SetModByType(xVehicle veh, VehicleModType modType, byte id);
  Task<xVehicle> CreateVehicle(string model, Position position, Rotation rotation);
  List<xVehicle> GetVehiclesInRadius(Position position, int range = 5);
}