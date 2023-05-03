using server.Core;

namespace server.Handlers.Player;

public interface IPlayerHandler
{
    Task<xPlayer> LoadPlayerFromDatabase(xPlayer player);
}