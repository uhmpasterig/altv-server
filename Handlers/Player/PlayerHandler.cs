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
using System.Diagnostics;

namespace server.Handlers.Player;

public class PlayerHandler : IPlayerHandler, IPlayerConnectEvent, IPlayerDisconnectEvent
{
  public static List<xPlayer> Players = new List<xPlayer>();
  IStorageHandler _storageHandler = new StorageHandler();
  ServerContext _serverContext = new ServerContext();

  #region Player Functions
  public async Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player)
  {
    try
    {
      Models.Player? dbPlayer = await _serverContext.Players
        .FirstOrDefaultAsync(p => p.name == player.Name);
      if (dbPlayer == null)
      {
        return null;
      }

      // PLAYER INFO
      dbPlayer.lastLogin = DateTime.Now;
      player.id = dbPlayer.permaId;
      player.name = dbPlayer.name;
      player.creationDate = dbPlayer.creationDate;

      // CACHE DATA
      dbPlayer.dataCache["isOnline"] = true;
      player.dataCache = dbPlayer.dataCache;

      // STORAGES
      player.playerInventorys = dbPlayer.playerInventorys;

      await _storageHandler.CreateAllStorages(player);

      if (player.playerInventorys.Count != dbPlayer.playerInventorys.Count)
      {
        dbPlayer.playerInventorys = player.playerInventorys;
      }

      foreach (var playerInventory in player.playerInventorys)
      {
        await _storageHandler.LoadStorage(playerInventory.Value);
      }

      player.cash = dbPlayer.cash;
      player.bank = dbPlayer.bank;

      // JOB
      player.job = dbPlayer.job;
      player.job_rank = dbPlayer.job_rank;
      player.job_perm = JsonConvert.DeserializeObject<List<string>>(dbPlayer.job_perm)!;

      // SPAWN AND SET PED VALUES
      player.Model = (uint)Alt.Hash("mp_m_freemode_01");
      player.Spawn(dbPlayer.Position, 0);
      player.Rotation = dbPlayer.Rotation;
      player.Health = dbPlayer.health;
      player.Armor = dbPlayer.armor;

      // WEAPONS
      player.LoadWeaponsFromDb(dbPlayer._weapons);

      player.Emit("player:loaded", new PlayerLoadedWriter(player));
      dbPlayer.isOnline = true;
      await _serverContext.SaveChangesAsync();
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
    Models.Player? dbPlayer = await _serverContext.Players.FindAsync(player.id);

    // PLAYER INFO
    dbPlayer.isOnline = false;
    dbPlayer.lastLogin = DateTime.Now;

    // CACHE DATA
    dbPlayer.dataCache = player.dataCache;

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

  public async Task<xPlayer?> GetPlayer(int id)
  {
    xPlayer? player = Players.Find(p => p.id == id);
    return player!;
  }

  #endregion Player Functions

  #region Player Events

  public async void OnPlayerConnect(IPlayer iplayer, string reason)
  {
    // new stopwatch to measure the time it takes to load the player
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

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

    stopwatch.Stop();
    _logger.Info($"Player {xplayer.Name} loaded in {stopwatch.ElapsedMilliseconds}ms (~{stopwatch.ElapsedTicks} Ticks)!");
  }

  public async void OnPlayerDisconnect(IPlayer player, string reason)
  {
    if (Players.Find(p => p.id == ((xPlayer)player).id) == null) return;
    _logger.Info($"{player.Name} disconnected from the server!");
    await SavePlayerToDatabase((xPlayer)player, true);
    Players.Remove((xPlayer)player);
  }

  #endregion Player Events
}
