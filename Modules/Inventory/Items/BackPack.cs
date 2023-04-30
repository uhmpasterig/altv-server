/* using server.Core;
using server.Events;

using server.Handlers.Storage;
using server.Config;
using Newtonsoft.Json;

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
      if(player.dataCache.ContainsKey("backpack")) {
        await PackBackPack(player);
      };

      xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
      storage.maxWeight = backpackWeight["big-backpack"];
      storage.slots = backpackSlots["big-backpack"];
      player.dataCache.Add("backpack", "big-backpack");
    });

    Items.RegisterUsableItem("small-backpack", async (xPlayer player) =>
    {
      if(player.dataCache.ContainsKey("backpack")) {
        await PackBackPack(player);
      };
      
      xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
      storage.maxWeight = backpackWeight["small-backpack"];
      storage.slots = backpackSlots["small-backpack"];
      player.dataCache.Add("backpack", "small-backpack");
    });

  }

  public static async Task PackBackPack(xPlayer player)
  {
    _logger.Log(JsonConvert.SerializeObject(player.dataCache));
    string backpack = (string)player.dataCache.Where(x => x.Key == "backpack").FirstOrDefault().Value;
    if (backpack == null) return;
    if (backpack != "small-backpack" && backpack != "big-backpack") return;
    player.dataCache.Remove("backpack");
    xStorage storage = await storageHandler.GetStorage(player.boundStorages["Inventar"]);
    storage.maxWeight = StorageConfig.StoragesDieJederHabenSollte.Where(x => x.name == "Inventar").FirstOrDefault().maxWeight;
    storage.slots = StorageConfig.StoragesDieJederHabenSollte.Where(x => x.name == "Inventar").FirstOrDefault().slots;
    await player.GiveItem(backpack, 1);
  }
} */