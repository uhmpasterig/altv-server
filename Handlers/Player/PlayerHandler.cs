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
  IStorageHandler _storageHandler = new StorageHandler();
  ServerContext _serverContext = new ServerContext();

  public async Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player)
  {
    try
    {
      Models.Player? dbPlayer = await _serverContext.Player
        .FirstOrDefaultAsync(p => p.name == player.Name);
      if (dbPlayer == null)
      {
        return null;
      }

      // PLAYER INFO
      dbPlayer.isOnline = true;
      dbPlayer.lastLogin = DateTime.Now;
      player.id = dbPlayer.permaId;
      player.name = dbPlayer.name;
      player.creationDate = dbPlayer.creationDate;

      // STORAGES
      player.playerInventorys = dbPlayer.playerInventorys;

      await _storageHandler.CreateAllStorages(player);
      _logger.Info($"Player {player.Name} loaded all storages");
      _logger.Info(JsonConvert.SerializeObject(player.playerInventorys));

      foreach (var playerInventory in player.playerInventorys)
      {
        _logger.Log("hey");
        _logger.Info($"Loading storage {playerInventory.Value}");
        await _storageHandler.LoadStorage(playerInventory.Value);
      }

      // JOB
      player.job = dbPlayer.job;
      player.job_rank = dbPlayer.job_rank;
      player.job_perm = JsonConvert.DeserializeObject<Dictionary<string, bool>>(dbPlayer.job_perm)!;

      // SPAWN AND SET PED VALUES
      player.Model = (uint)Alt.Hash("mp_m_freemode_01");
      player.Spawn(dbPlayer.Position, 0);
      player.Rotation = dbPlayer.Rotation;
      player.Health = dbPlayer.health;
      player.Armor = dbPlayer.armor;

      // WEAPONS
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
    Models.Player? dbPlayer = await _serverContext.Player.FindAsync(player.id);

    // PLAYER INFO
    dbPlayer.lastLogin = DateTime.Now;
    dbPlayer.isOnline = true;

    // STORAGES
    foreach (var playerInventory in player.playerInventorys)
    {
      await _storageHandler.UnloadStorage(playerInventory.Value);
    }

    // PED VALUES
    if (player.Dimension == 0) dbPlayer.Position = player.Position;
    dbPlayer.Rotation = player.Rotation;
    dbPlayer.health = player.Health;
    dbPlayer.armor = player.Armor;

    // WEAPONS
    dbPlayer._weapons = JsonConvert.SerializeObject(player.weapons);

    if (isDisconnect)
    {
      dbPlayer.isOnline = false;
      player.Kick("Du wurdest gekickt!");
    }
    await _serverContext.SaveChangesAsync();
  }

  public async Task SaveAllPlayers()
  {
    foreach (xPlayer player in Players)
    {
      await SavePlayerToDatabase(player);
    }
    await _serverContext.SaveChangesAsync();
  }

  public async void OnPlayerConnect(IPlayer iplayer, string reason)
  {
    xPlayer? xplayer = await LoadPlayerFromDatabase((xPlayer)iplayer);

    if (xplayer == null)
    {
      iplayer.Kick("Du bist nicht in der Datenbank! " +
        "Falls du bereits registriert bist, versuche Vor_Nachname als Launchername zu benutzen. " +
        "Falls du noch nicht registriert bist, registriere dich auf unserem Discord!");
      return;
    }
    _logger.Info($"{xplayer.Name} connected to the server!");
    Players.Add(xplayer);
  }

  public async void OnPlayerDisconnect(IPlayer player, string reason)
  {
    if (Players.Find(p => p.id == ((xPlayer)player).id) == null) return;
    _logger.Info($"{player.Name} disconnected from the server!");
    await SavePlayerToDatabase((xPlayer)player, true);
    Players.Remove((xPlayer)player);
  }

  public async Task<xPlayer> GetPlayer(int id)
  {
    xPlayer? player = Players.Find(p => p.id == id);
    return player!;
  }
}
