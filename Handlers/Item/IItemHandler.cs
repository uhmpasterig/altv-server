using server.Core;
using server.Models;

namespace server.Handlers.Items;
public interface IItemHandler
{
  Task RegisterUseableItem(string itemname, Action<xPlayer, Dictionary<string, object>, Action> action);
  Task<bool> ItemExists(int id);
  Task<Item> GetItem(int id);
  Task<Item> GetItem(string name);
  Task UseItem(xPlayer player, string name, int slot = 0);
  Task<List<Storage_Item>> CreateItemStacks(Item _item, int count);
}