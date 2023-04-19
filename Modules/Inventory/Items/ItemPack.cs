using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Modules.Items;

class UsabelMedikit : IItemsLoaded
{
  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("medikit", async (xPlayer player) =>
    {
      player.Emit("playAnim", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", -1, 1);
      await Task.Delay(5000);
      player.Emit("stopAnim");
      player.Health = player.MaxHealth;
    });

    Items.RegisterUsableItem("weste", async (xPlayer player) =>
    {
      player.Emit("playAnim", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", -1, 1);
      await Task.Delay(5000);
      player.Emit("stopAnim");
      player.Armor = 100;
    });
  }
}