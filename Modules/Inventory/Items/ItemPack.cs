using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Modules.Items;

class UsabelMedikit : IItemsLoaded, IPressedEEvent
{
  public static string usedItems { get; set; }
  public static Dictionary<xPlayer, Dictionary<int, string>> playerUsedItems  = new Dictionary<xPlayer, Dictionary<int, string>>();

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    _logger.Debug("OnKeyPressE");
    if(playerUsedItems.ContainsKey(player) == false) return false;
    _logger.Debug("OnKeyPressE2");
    
    foreach (KeyValuePair<int, string> item in playerUsedItems[player])
    {
      _logger.Exception(item.Value);
      player.GiveItem(item.Value, 1);
      playerUsedItems[player][item.Key] = "";
    }
    player.Emit("stopAnim");
    return true;
  }

  static int DefaultFunc(xPlayer player, string item)
  {
    int randomNumber = new Random().Next(0, 99999);
    if (playerUsedItems.ContainsKey(player) == false)
    {
      playerUsedItems.Add(player, new Dictionary<int, string>());
    }
    playerUsedItems[player].Add(randomNumber, item);
    return randomNumber;
  }

  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("medikit", async (xPlayer player) =>
    {
      int randomNumber = DefaultFunc(player, "medikit");
      player.Emit("playAnim", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", -1, 1);
      await Task.Delay(5000);

      if (playerUsedItems[player][randomNumber] == "")
      {
        playerUsedItems[player].Remove(randomNumber);
        return;
      };

      player.Emit("stopAnim");
      player.Health = player.MaxHealth;
    });

    Items.RegisterUsableItem("weste", async (xPlayer player) =>
    {
      int randomNumber = DefaultFunc(player, "weste");
      player.Emit("playAnim", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", -1, 1);
      await Task.Delay(5000);

      if (playerUsedItems[player][randomNumber] == "")
      {
        playerUsedItems[player].Remove(randomNumber);
        return;
      };

      player.Emit("stopAnim");
      player.Armor = 100;
    });
  }
}