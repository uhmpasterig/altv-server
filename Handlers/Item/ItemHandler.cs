using server.Core;
using server.Models;
using server.Events;
using AltV.Net.Async;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;

namespace server.Handlers.Items;
public class ItemHandler : IItemHandler, ILoadEvent
{
  public static IItemHandler Instance => new ItemHandler();
  
  ServerContext _itemCtx = new ServerContext();
  IStorageHandler _storageHandler = new StorageHandler();


  public static List<Item> Items = new List<Item>();
  public static Dictionary<int, Action<xPlayer, Dictionary<string, object>, Action>> ItemActions = new Dictionary<int, Action<xPlayer, Dictionary<string, object>, Action>>();

  public ItemHandler()
  {
  }

  public async void OnLoad()
  {
    Items = _itemCtx.Items.ToList();
    _logger.Startup($"Loaded x{Items.Count} items into memory.");
    AltAsync.Emit("items:loaded");
  }

  public async Task RegisterUseableItem(string itemname, Action<xPlayer, Dictionary<string, object>, Action> action)
  {
    Item? item = await GetItem(itemname);
    if (item == null) {
      _logger.Exception($"Cannot register useable item {itemname} because it does not exist. Trace: {Environment.StackTrace}");
    };
    ItemActions.Add(item.id, action);
  }

  public async Task<bool> ItemExists(int id)
  {
    return Items.Where(i => i.id == id).FirstOrDefault() != null;
  }

  public async Task<Item?> GetItem(int id)
  {
    return Items.Where(i => i.id == id).FirstOrDefault();
  }

  public async Task<Item?> GetItem(string name)
  {
    return Items.Where(i => i.name == name).FirstOrDefault();
  }

  public async Task UseItem(xPlayer player, string name, int slot = 0)
  {
    Item? item = await GetItem(name);
    if (item == null) return;
    xStorage? inventory = await _storageHandler.GetStorage(player.boundStorages[1]);
    if (inventory == null) return;
    if (!await inventory.ContainsItem(item.name)) return;
    Storage_Item? storageItem = null;

    if (slot == 0)
      storageItem = await inventory.GetItem(item.name);
    else
      storageItem = await inventory.GetItem(slot);

    if (storageItem == null) return;

    Action RemoveItem = async () =>
    {
      await inventory.RemoveItem(item.name, 1);
    };

    if (ItemActions.ContainsKey(item.id))
    {
      ItemActions[item.id](player, storageItem.Item_Data.data, RemoveItem);
      return;
    }

    _logger.Debug($"Item {item.name} does not have an action.");
  }

  public async Task<List<Storage_Item>> CreateItemStacks(Item _item, int count)
  {
    List<Storage_Item> stacks = new List<Storage_Item>();
    while(count > 0){
      int stackCount = count > _item.stackSize ? _item.stackSize : count;
      count -= stackCount;
      stacks.Add(new Storage_Item(_item, stackCount));
    }
    return stacks;
  }
}