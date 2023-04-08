using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Modules.Items;

class UsabelMedikit : IItemsLoaded
{
  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("medikit", (xPlayer player) =>
    {
      _logger.Debug("Medikit used");
    });

    Items.RegisterUsableItem("weste", (xPlayer player) =>
    {
      _logger.Debug("Weste used");
    });
  }
}