using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;

namespace server.Handlers.Storage;

public interface IStorageHandler
{
  Task<xStorage> GetStorage(int storageId);
  Task<bool> LoadStorage(int storageId);
  Task UnloadStorage(int storageId);
  Task<int> CreateStorage(string name, int slots, float maxWeight);
  xStorage GetClosestxStorage(xPlayer player, int range = 2);
  Task SaveAllStorages();
}