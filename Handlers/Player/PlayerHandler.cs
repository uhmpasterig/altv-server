using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using server.Core;
using server.Models;
using Microsoft.EntityFrameworkCore;
using _logger = server.Logger.Logger;
using server.Events;
using server.Handlers.Storage;
using Newtonsoft.Json;
using server.Util.Player;

namespace server.Handlers.Player;

public class PlayerHandler : IPlayerHandler, IPlayerConnectEvent, IPlayerDisconnectEvent
{
  public static List<xPlayer> Players = new List<xPlayer>();
  IStorageHandler storageHandler = new StorageHandler();

  public async Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player)
  {
    try
    {
      await using var serverContext = new ServerContext();
      Models.Player? dbPlayer = await serverContext.Player
        .FirstOrDefaultAsync(p => p.name == player.Name);
      if (dbPlayer == null)
      {
        _logger.Debug($"{player.Name} is not in the db!");
        return null;
      }
      dbPlayer.isOnline = true;
      dbPlayer.lastLogin = DateTime.Now;

      player.id = dbPlayer.permaId;
      player.name = dbPlayer.name;
      player.creationDate = dbPlayer.creationDate;

      // STORAGES
      player.playerInventorys = dbPlayer.playerInventorys;
      foreach (var playerInventory in player.playerInventorys)
      {
        string name = playerInventory.Key;
        int storageId = playerInventory.Value;
        _logger.Debug($"Requesting inv {name}!");
        await storageHandler.LoadStorage(storageId);
      }
      _logger.Debug($"{player.name} is loaded from the db!");
      _logger.Debug($"{player.name} has the id: {player.id}!");
      _logger.Debug($"{player.name} has the last login: {dbPlayer.lastLogin}!");

      player.job = dbPlayer.job;
      player.job_rank = dbPlayer.job_rank;
      player.job_perm = JsonConvert.DeserializeObject<Dictionary<string, bool>>(dbPlayer.job_perm)!;

      player.Model = (uint)Alt.Hash("mp_m_freemode_01");

      player.Spawn(dbPlayer.Position, 0);
      player.Rotation = dbPlayer.Rotation;
      player.SetWeather(AltV.Net.Enums.WeatherType.ExtraSunny);
      player.SetDateTime(DateTime.Now);

      player.Health = dbPlayer.health;
      player.Armor = dbPlayer.armor;
      player.LoadWeaponsFromDb(dbPlayer._weapons);

      player.Emit("player:loaded", new PlayerLoadedWriter(player));

      return player;
    }
    catch (Exception e)
    {
      _logger.Exception(e.Message);
      return null;
    }
  }

  public async Task SavePlayerToDatabase(xPlayer player, bool isDisconnect = false)
  {
    await using var serverContext = new ServerContext();
    Models.Player? dbPlayer = await serverContext.Player.FindAsync(player.id);
    dbPlayer!.lastLogin = DateTime.Now;
    if (player.Dimension == 0)
    {
      dbPlayer.Position = player.Position;
    }

    dbPlayer._weapons = JsonConvert.SerializeObject(player.weapons);
    dbPlayer.Rotation = player.Rotation;

    dbPlayer.health = player.Health;
    dbPlayer.armor = player.Armor;

    if (isDisconnect)
    {
      dbPlayer.isOnline = false;
      player.Kick("Du wurdest gekickt!");
    }
    await serverContext.SaveChangesAsync();
  }

  public async Task SaveAllPlayers()
  {
    await using var serverContext = new ServerContext();
    foreach (xPlayer player in Players)
    {
      await SavePlayerToDatabase(player);
    }
    await serverContext.SaveChangesAsync();
  }

  public async Task OnPlayerConnect(IPlayer iplayer, string reason)
  {
    xPlayer? xplayer = await LoadPlayerFromDatabase((xPlayer)iplayer);
    Players.Add(xplayer!);
  }

  public async Task OnPlayerDisconnect(IPlayer player, string reason)
  {
    _logger.Info($"{player.Name} disconnected from the server!");
    await SavePlayerToDatabase((xPlayer)player, true);
    Players.Remove((xPlayer)player);
  }

  public xPlayer GetPlayer(int id)
  {
    xPlayer? player = Players.Find(p => p.id == id);
    return player!;
  }
}
