using AltV.Net;
using server.Core;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using AltV.Net.Data;
using server.Modules.Items;

namespace server.Core;

public class xStorage : Models.Storage
{
  public List<InventoryItem> items = new List<InventoryItem>();
  public float weight = 0;

  public xStorage(Models.Storage storage)
  {
    this.id = storage.id;
    this.name = storage.name;
    this.maxWeight = storage.maxWeight;
    this.currentWeight = storage.currentWeight;
    this.slots = storage.slots;

    this._pos = storage._pos;
    this.usePos = storage.usePos;

    this._items = storage._items;
    this.items = JsonConvert.DeserializeObject<List<InventoryItem>>(this._items);
    _logger.Log($"Storage {this.name} loaded");
    _logger.Log(JsonConvert.SerializeObject(this.items));
    this.CalculateWeight();
  }

  public void CalculateWeight()
  {
    this.weight = 0;
    foreach (var item in this.items)
    {
      this.weight += item.weight * item.count;
    }
  }

  public Dictionary<int, int> GetIncompleteStacks(string itemname)
  {
    Dictionary<int, int> stacks = new Dictionary<int, int>();
    foreach (var item in this.items)
    {
      if (item.name == itemname)
      {
        if (item.count < item.stackSize)
        {
          _logger.Log($"Adding incomplete stack {item.slot} with {item.stackSize - item.count} items");
          stacks.Add(item.slot, item.stackSize - item.count);
        }
      }
    }
    return stacks;
  }

  public int GetFreeSlot()
  {
    for (int i = 1; i <= this.slots; i++)
    {
      if (!this.items.Any(x => x.slot == i))
      {
        _logger.Log($"Found free slot {i}");
        return i;
      }
    }
    return -1;
  }

  public void AddItem(string itemname, int count = 1)
  {
    xItem item = Items.GetItem(itemname);
    if (this.items.Count > this.slots)
    {
      _logger.Error($"Storage {this.name} is full");
      return;
    }
    if (this.weight + item.weight > this.maxWeight)
    {
      _logger.Error($"Storage {this.name} is full");
      return;
    }
    Dictionary<int, int> stacks = this.GetIncompleteStacks(itemname);
    int toAdd = count;
    foreach (KeyValuePair<int, int> stack in stacks)
    {
      toAdd -= stack.Value;
    }
    if (toAdd > 0)
    {
      int slotsNeeded = (int)Math.Ceiling((double)toAdd / item.stackSize);
      if (this.items.Count + slotsNeeded > this.slots)
      {
        _logger.Error($"Storage {this.name} is full");
        return;
      }
    }

    // Add items to existing stacks
    foreach (KeyValuePair<int, int> stack in stacks)
    {
      InventoryItem alrItem = this.items.FirstOrDefault(x => x.slot == stack.Key);
      _logger.Log($"Adding {count} to stack {stack.Key}, stack size: {stack.Value}");
      if (count <= stack.Value)
      {
        _logger.Log($"1Adding {count} to stack {stack.Key}");
        alrItem.count += count;
        goto itemFinish;
      }
      else
      {
        _logger.Log($"2Adding {stack.Value} to stack {stack.Key}");
        alrItem.count += stack.Value;
        count -= stack.Value;
      }
    }
    if (count == 0) goto itemFinish;
    // Add items to new stacks
    while (count > item.stackSize)
    {
      InventoryItem iitem = new InventoryItem(item, this.GetFreeSlot(), item.stackSize);
      _logger.Log($"Adding {item.stackSize} to new stack {iitem.slot}");
      this.items.Add(iitem);
      count -= item.stackSize;
    }
  newItem:
    if (count > 0)
    {
      InventoryItem iitem = new InventoryItem(item, this.GetFreeSlot(), count);
      _logger.Log($"Adding {count} to new stack {iitem.slot}");
      this.items.Add(iitem);
    }

  itemFinish:

    this.CalculateWeight();
  }

  public InventoryItem GetItemFromSlot(int slot)
  {
    return this.items.FirstOrDefault(x => x.slot == slot);
  }

  public async Task<bool> DragAddItem(InventoryItem item)
  {
    return true;
  }

  public void DragRemoveItem(int slot)
  {
    InventoryItem item = this.items.FirstOrDefault(x => x.slot == slot)!;
    this.items.Remove(item);
  }


  public object GetData()
  {
    object data = new
    {
      id = this.id,
      name = this.name,
      weight = this.weight,
      maxWeight = this.maxWeight,
      currentWeight = this.currentWeight,
      slots = this.slots,
      items = this.items
    };
    return data;
  }

  public Position Position
  {
    get
    {
      return JsonConvert.DeserializeObject<Position>(_pos);
    }
    set
    {
      _pos = JsonConvert.SerializeObject(value);
    }
  }
}