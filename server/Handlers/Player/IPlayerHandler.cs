using server.Core;

namespace server.Handlers.Player;

public interface IPlayerHandler
{
    Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player);
    Task SavePlayerToDatabase(xPlayer player, bool isDisconnect = false, bool isKick = false);
    Task SaveAllPlayers();
    Task<xPlayer?> GetPlayer(int playerId);
}