using server.Core;

namespace server.Handlers.Player;

public interface IPlayerHandler
{
    Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player);
    Task SavePlayerToDatabase(xPlayer player, bool isDisconnect = false);
    Task SaveAllPlayers();
    xPlayer? GetPlayer(int playerId);
}