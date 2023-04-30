using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;

namespace server.Handlers.Storage;

public interface IStorageHandler
{
  Task<xStorage?> GetStorage(int id);
  Task<bool> LoadStorage(int id);
  Task UnloadStorage(int id);
  Task UnloadStorage(xStorage storage);
  Task<int> CreateStorage(string name, int slots, float maxWeight, Position? position, int ownerId = -1);
  Task SaveAllStorages();
  Task CreateAllStorages(xPlayer player);
  Task<xStorage?> GetClosestStorage(xPlayer player, int range = 2);

}