using server.Core;
using server.Models;
using AltV.Net.Data;

namespace server.Handlers.Vehicle;

public interface IVehicleHandler
{
  /* Task<xVehicle> CreateVehicle(int ownerId, int garageId, Position position, Rotation rotation);

  Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle);
  Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle, Position position, Rotation rotation);

  Task SaveAllVehicles(); */
  Task SaveAllVehicles();
  xVehicle GetClosestxVehicle(Position position, int range = 2);
}