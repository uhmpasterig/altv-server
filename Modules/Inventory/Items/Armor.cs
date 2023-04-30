using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Modules.Items;

class Westen : IItemsLoaded
{
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
      await player.GiveItem(player.dataCache["weste"].ToString()!, 1, new Dictionary<string, object> { { "value", armor } });
      player.dataCache.Remove("weste");
    }
  }

  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("underarmor", async (xPlayer player) =>
    {
      player.Emit("playAnim", "items_weste");
      player.Emit("clientProgressbarStart", time);
      await Task.Delay(time);
      player.Emit("stopAnim");
      player.maxArmor = 75;
      player.Armor = 75;
      player.dataCache.Add("weste", "underarmor");
    });

    Items.RegisterUsableItem("halfplate", async (xPlayer player) =>
    {
      player.Emit("playAnim", "items_weste");
      player.Emit("clientProgressbarStart", time);
      await Task.Delay(time);
      player.Emit("stopAnim");
      player.maxArmor = 100;
      player.Armor = 100;
      player.dataCache.Add("weste", "halfplate");
    });

    Items.RegisterUsableItem("fullplate", async (xPlayer player) =>
    {
      player.Emit("playAnim", "items_weste");
      player.Emit("clientProgressbarStart", time);
      await Task.Delay(time);
      player.Emit("stopAnim");
      player.maxArmor = 125;
      player.Armor = 125;
      player.dataCache.Add("weste", "fullplate");
    });
  }
}