using server.Core;
using server.Models;
using AltV.Net.Data;
using AltV.Net.Enums;

namespace server.Handlers.Vehicle;

public interface IVehicleHandler
{

  Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle);
  Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle, Position position, Rotation rotation);

  Task SaveAllVehicles();
  Task<xVehicle> GetClosestVehicle(Position position, int range = 2);
  bool SetModByType(xVehicle veh, VehicleModType modType, byte id);
  Task<xVehicle> CreateVehicle(string model, Position position, Rotation rotation);
  Task<List<xVehicle>> GetVehiclesInRadius(Position position, int range = 5);
  Task<xVehicle> GetVehicle(int id);
  Task<List<Models.Vehicle>> GetVehiclesInGarage(xPlayer player, int garageId);
  Task SaveDbVehicleInGarage(xVehicle vehicle, int garage_id);
}