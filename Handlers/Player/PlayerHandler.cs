using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using server.Core;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Events;
using Newtonsoft.Json;
using server.Util.Player;
using System.Diagnostics;
using server.Config;
using server.Modules.Factions;

using server.Handlers.Storage;
using server.Handlers.Logger;

namespace server.Handlers.Player;

public class PlayerHandler : IPlayerHandler, IPlayerConnectEvent, IPlayerDisconnectEvent, IOneMinuteUpdateEvent, ITwoMinuteUpdateEvent, IFiveSecondsUpdateEvent
{

  public ServerContext _playerCtx = new ServerContext();
  public Dictionary<int, xPlayer> Players = new Dictionary<int, xPlayer>();

  ILogger _logger;
  IStorageHandler _storageHandler;
  public PlayerHandler(ILogger logger, IStorageHandler storageHandler)
  {
    _logger = logger;
    _storageHandler = storageHandler;
  }

  #region Player Functions
  public async Task<xPlayer?> LoadPlayerFromDatabase(xPlayer player)
  {
    try
    {
      Models.Player? dbPlayer = await _playerCtx.Players
        .Include(p => p.player_skin)
        .Include(p => p.player_cloth)
        .Include(p => p.vehicle_keys)

        .Include(p => p.player_society)
        .Include(p => p.player_society.Faction)
        .Include(p => p.player_society.Business)
        .Include(p => p.player_factory)

        .FirstOrDefaultAsync(p => p.name == player.Name);

      if (dbPlayer == null) return null;
      await player.SetDataFromDatabase(dbPlayer);

      // STORAGES
      await _storageHandler.CreateAllStorages(player);
      if (player.boundStorages.Count != dbPlayer.boundStorages.Count) dbPlayer.boundStorages = player.boundStorages;
      foreach (KeyValuePair<int, int> storage in player.boundStorages)
        if (StorageConfig.StoragesDieJederHabenSollte.Where(s => s.local_id == storage.Key).FirstOrDefault().loadOnConnect) await _storageHandler.LoadStorage(storage.Value);

      // SPAWN AND SET PED VALUES
      player.Model = (uint)Alt.Hash(player.ped);
      player.Spawn(dbPlayer.Position, 0);

      await player.LoadSkin();
      await player.LoadClothes();

      player.Rotation = dbPlayer.Rotation;
      player.Health = dbPlayer.health;
      player.Armor = dbPlayer.armor;
      player.MaxArmor = dbPlayer.max_armor;

      // WEAPONS
      player.LoadWeaponsFromDb(dbPlayer._weapons);

      player.Emit("player:loaded", new PlayerLoadedWriter(player));
      dbPlayer.lastLogin = DateTime.Now;
      dbPlayer.isOnline = true;

      await _playerCtx.SaveChangesAsync();
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
    Models.Player? dbPlayer = await _playerCtx.Players.FindAsync(player.id);

    // PLAYER INFO
    dbPlayer.isOnline = false;
    dbPlayer.lastLogin = DateTime.Now;

    // CACHE DATA
    dbPlayer.dataCache = player.dataCache;

    // STORAGES
    foreach (var playerInventory in player.boundStorages)
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
    await _playerCtx.SaveChangesAsync();
  }

  public async Task SaveAllPlayers()
  {
    foreach (xPlayer player in Players.Values)
    {
      await SavePlayerToDatabase(player);
    }
    await _playerCtx.SaveChangesAsync();
  }

  public async Task<xPlayer?> GetPlayer(int id)
  {
    if (!Players.ContainsKey(id)) return null;
    xPlayer? player = Players[id];
    return player;
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
    Players.Add(xplayer.id, xplayer);

    stopwatch.Stop();
    _logger.Info($"Player {xplayer.Name} loaded in {stopwatch.ElapsedMilliseconds}ms (~{stopwatch.ElapsedTicks} Ticks)!");
  }

  public async void OnPlayerDisconnect(IPlayer player, string reason)
  {
    xPlayer xplayer = (xPlayer)player;
    if (!Players.ContainsKey(xplayer.id)) return;
    _logger.Info($"{player.Name} disconnected from the server!");
    await SavePlayerToDatabase((xPlayer)player, true);
    Players.Remove(xplayer.id);
  }

  public async Task<int> CalculateSocialGrade(int playtime)
  {
    int hours = playtime / 60;
    int grade = 0;
    // from grade 1 to 2 it takes one hour from 2 - 3 it takes 2 hours and so on the max time per grade is 100 hours
    return grade;
  }

  public async void OnFiveSecondsUpdate()
  {
    _playerCtx.Players.Where(p => p.isOnline == true).ToList().ForEach(async p =>
    {
      xPlayer? player = await GetPlayer(p.id);
      if (player.Dimension == (int)DIMENSIONEN.WORLD)
      {
        p.Position = player.Position;
        p.Rotation = player.Rotation;
      }
    });
    _playerCtx.SaveChanges();
  }

  public async void OnOneMinuteUpdate()
  {
    ServerContext __playerCtx = new ServerContext();
    __playerCtx.Players
      .Include(p => p.player_society)
      .Where(p => p.isOnline)
      .ToList()
    .ForEach(async p =>
    {
      p.player_society.playtime += 1;
      p.player_society.grade = await CalculateSocialGrade(p.player_society.playtime);
    });
    _logger.Info("Saved all players!");
    __playerCtx.SaveChanges();
  }

  public async void OnTwoMinuteUpdate()
  {
    await _playerCtx.SaveChangesAsync();
  }

  #endregion Player Events
}
