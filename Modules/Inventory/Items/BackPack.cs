using server.Core;
using server.Events;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;

namespace server.Modules.Items;

class BackPacks : IItemsLoaded
{
  IStorageHandler storageHandler = new StorageHandler();
  public void ItemsLoaded()
  {
    Dictionary<string, int> backpackSlots = new Dictionary<string, int>()
    {
      { "small-backpack", 12 },
      { "big-backpack", 14 },
    };

    Dictionary<string, int> backpackWeight = new Dictionary<string, int>()
    {
      { "small-backpack", 200 },
      { "big-backpack", 250 },
    };

    Items.RegisterUsableItem("big-backpack", async (xPlayer player) =>
    {
      xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
      storage.maxWeight = backpackWeight["big-backpack"];
      storage.slots = backpackSlots["big-backpack"];
    });

    Items.RegisterUsableItem("small-backpack", async (xPlayer player) =>
    {
      xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
      storage.maxWeight = backpackWeight["small-backpack"];
      storage.slots = backpackSlots["small-backpack"];
    });
  }
}