using server.Core;
using server.Events;

using server.Handlers.Items;

namespace server.Modules.Items;

class Westen : IItemsLoaded
{
  IItemHandler itemHandler = ItemHandler.Instance;

  static int time = 5000;
  public static async void PackWeste(xPlayer player)
  {
    if (player.dataCache.ContainsKey("weste"))
    {
      player.Emit("playAnim", "items_weste_pack");
      player.Emit("clientProgressbarStart", time);
      await Task.Delay(time);
      player.Emit("stopAnim");

      int armor = player.Armor;
      player.maxArmor = 100;
      player.Armor = 0;
      // TODO: Fix this
      // await player.GiveItem(player.dataCache["weste"].ToString()!, 1, new Dictionary<string, object> { { "value", armor } });
      player.dataCache.Remove("weste");
    }
  }
  
  public void ItemsLoaded()
  {
    itemHandler.RegisterUseableItem("underarmor", async (xPlayer player, Dictionary<string, object> data, Action RemoveItem) =>
    {
      player.Emit("playAnim", "items_weste");
      player.Emit("clientProgressbarStart", time);
      await Task.Delay(time);
      player.Emit("stopAnim");
      RemoveItem();
      player.maxArmor = 75;
      player.Armor = 75;
      player.dataCache.Add("weste", "underarmor");
    });
  }
}