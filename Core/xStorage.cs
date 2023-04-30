using AltV.Net;
using server.Core;
using server.Models;

using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using AltV.Net.Data;
using server.Handlers.Items;


namespace server.Core;

public class xStorage : Models.Storage
{
  public float weight = 0;

  public List<xPlayer> PlayersInside = new List<xPlayer>();

  public xStorage(Models.Storage storage)
  {
    this.id = storage.id;
    this.name = storage.name;
    this.maxWeight = storage.maxWeight;
    this.currentWeight = storage.currentWeight;
    this.slots = storage.slots;
    this.ownerId = storage.ownerId;

    this._pos = storage._pos;
    this.usePos = storage.usePos;
    this.Items = storage.Items;
    this.CalculateWeight();
  }

  #region Methods

  public void CalculateWeight()
  {
    this.weight = 0;
    foreach (Storage_Item item in this.Items)
    {
      this.weight += item.Item_Data.weight * item.count;
    }
  }

  public async Task<List<Storage_Item>> GetStacks(string name)
  {
    List<Storage_Item> stacks = new List<Storage_Item>();
    foreach (Storage_Item item in this.Items.OrderByDescending(x => x.slot))
    {
      if (item.Item_Data.name == name && item.count < item.Item_Data.stackSize)
      {
        stacks.Add(item);
      }
    }
    return stacks;
  }

  public async Task<int> GetAmount(string name)
  {
    int amount = 0;
    foreach (Storage_Item item in this.Items)
    {
      if (item.Item_Data.name == name)
      {
        amount += item.count;
      }
    }
    return amount;
  }

  public async Task<bool> ContainsItem(string name, int count = 1)
  {
    int amount = await this.GetAmount(name);
    if (amount >= count)
    {
      return true;
    }
    return false;
  }

  public async Task<Storage_Item?> GetItem(int slot)
  {
    Storage_Item? item = this.Items.Find(x => x.slot == slot);
    return item;
  }

  public async Task<Storage_Item?> GetItem(string name)
  {
    Storage_Item? item = this.Items.OrderByDescending(x => x.slot).LastOrDefault(x => x.Item_Data.name == name);
    return item;
  }

  private async Task<List<int>> GetFreeSlots()
  {
    List<int> slots = new List<int>();
    for (int i = 1; i <= this.slots; i++)
    {
      if (this.Items.Find(x => x.slot == i) == null)
      {
        slots.Add(i);
      }
    }
    return slots;
  }

  private async Task<int> GetFreeSlot()
  {
    int slot = -1;
    for (int i = 1; i <= this.slots; i++)
    {
      if (this.Items.Find(x => x.slot == i) == null)
      {
        slot = i;
        break;
      }
    }
    return slot;
  }

  private async Task<int> FillStacks(string name, int count)
  {
    foreach (Storage_Item item in await this.GetStacks(name))
    {
      if (item.count < item.Item_Data.stackSize)
      {
        int free = item.Item_Data.stackSize - item.count;
        if (free >= count)
        {
          item.count += count;
          return 0;
        }
        else
        {
          item.count += free;
          count -= free;
        }
      }
    }
    return count;
  }

  public async Task<bool> CanFitItem(Storage_Item item)
  {
    if ((item.Item_Data.weight * item.count) + this.weight > this.maxWeight) return false;
    if (await this.GetFreeSlot() == -1) return false;
    return true;
  }

  public async Task<bool> CanFitItems(List<Storage_Item> items)
  {
    float weight = 0;
    List<int> freeSlots = await this.GetFreeSlots();
    foreach (Storage_Item item in items)
    {
      weight += item.Item_Data.weight * item.count;
    }
    if (weight + this.weight > this.maxWeight) return false;
    if (freeSlots.Count < items.Count) return false;
    return true;
  }

  #endregion

  public async Task<bool> AddItem(Storage_Item item)
  {
    // Defining needed variables
    int slot = await this.GetFreeSlot();

    // Checks for space and item
    if (item == null) return false;
    if (item.count <= 0) return false;
    if (!await this.CanFitItem(item)) return false;
    if (slot == -1) return false;

    // Adding item
    item.slot = slot;
    item.Storage = this;
    this.Items.Add(item);
    this.CalculateWeight();
    return true;
  }

  public async Task<bool> AddItem(string name, int count)
  {
    // Defining needed variables
    IItemHandler itemHandler = ItemHandler.Instance;
    Item item = await itemHandler.GetItem(name);
    if (item == null) return false;
    List<Storage_Item> items = await itemHandler.CreateItemStacks(item, count);

    // Checks for space and item
    if (items == null) return false;
    if (!await this.CanFitItems(items)) return false;

    // Adding items
    foreach (Storage_Item item_ in items)
    {
      await this.AddItem(item_);
    }
    return false;
  }

  public async Task<bool> RemoveItem(Storage_Item item)
  {
    if (item == null) return false;
    this.Items.Remove(item);
    this.CalculateWeight();
    return true;
  }

  public async Task<bool> RemoveItem(string name, int count)
  {
    // Defining needed variables
    IItemHandler itemHandler = ItemHandler.Instance;
    Item item = await itemHandler.GetItem(name);
    if (item == null) return false;
    List<Storage_Item> items = await itemHandler.CreateItemStacks(item, count);

    // Checks for space and item
    if (items == null) return false;
    if (!await this.ContainsItem(name, count)) return false;

    // Removing items
    foreach (Storage_Item item_ in items)
    {
      await this.RemoveItem(item_);
    }
    return false;
  }
}