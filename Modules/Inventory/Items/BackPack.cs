using server.Core;
using server.Events;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using server.Config;

namespace server.Modules.Items;

class BackPacks : IItemsLoaded
{
  public static IStorageHandler storageHandler = new StorageHandler();
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
      player.dataCache.Add("backpack", "big-backpack");
    });

    Items.RegisterUsableItem("small-backpack", async (xPlayer player) =>
    {
      xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
      storage.maxWeight = backpackWeight["small-backpack"];
      storage.slots = backpackSlots["small-backpack"];
      player.dataCache.Add("backpack", "small-backpack");
    });

  }

  public static async void PackBackPack(xPlayer player)
  {
    if (player.dataCache.TryGetValue("backpack", out var backpack))
    {
      if ((string)backpack != "small-backpack" && (string)backpack != "big-backpack") return;

      player.dataCache.Remove("backpack");
      xStorage storage = storageHandler.GetStorage(player.boundStorages["Inventar"]).Result;
      storage.maxWeight = StorageConfig.StoragesDieJederHabenSollte.Where(x => x.name == (string)backpack).FirstOrDefault().maxWeight;
      storage.slots = StorageConfig.StoragesDieJederHabenSollte.Where(x => x.name == (string)backpack).FirstOrDefault().slots;
      await player.GiveItem((string)backpack, 1);
    }
  }
}