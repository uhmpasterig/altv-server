using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using AltV.Net.Async;
using AltV.Net.Events;
using Newtonsoft.Json;
using System;
using System.Windows;

using server.Handlers.Sql;
using MySql.Data.MySqlClient;
using server.Entitys;

using server;
using server.Player;

using server.Data;
namespace server.Scripts
{
  public class ffa : IScript
  {
    class FFA
    {
      public int creator { get; set; }
      public int id { get; set; }
      public string name { get; set; }
      public string password { get; set; }
      public int maxPlayers { get; set; }
      public int players { get; set; }
      public string[] weapons { get; set; }
      public string map { get; set; }
      public int time { get; set; }
      public bool healPerKill { get; set; }
      public List<FFAPlayer> playersInFFA { get; set; }
    }
    class FFAPlayer
    {
      public int playerId { get; set; }
      public int kills { get; set; }
      public int deaths { get; set; }
      public Position cachedPosition { get; set; }
    }
    class FFAMap
    {
      public string name { get; set; }
      public Position[] spawns { get; set; }
    }
    static List<FFA> ffas = new List<FFA>();
    static List<FFAMap> ffaMaps = new List<FFAMap>();

    public static async void _init()
    {
      MySqlCommand command = Datenbank.Connection.CreateCommand();
      command.CommandText = "SELECT * FROM ffaMaps";
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        FFAMap map = new FFAMap();
        map.name = reader.GetString("name");
        map.spawns = JsonConvert.DeserializeObject<Position[]>(reader.GetString("spawns"));
        Alt.Log(map.name);
        ffaMaps.Add(map);
      }
      reader.Close();
    }

    [Command("createffa")]
    public void Createffa(IPlayer iplayer, string name, string password, string map, int maxPlayers, string weapon, string weapon2, bool healPerKill = false)
    {
      xPlayer player = Players.getPlayer(iplayer);

      FFA createFFA = new FFA();
      createFFA.creator = player.id;
      createFFA.id = ffas.Count + 1;
      createFFA.name = name;
      createFFA.password = password;
      createFFA.maxPlayers = maxPlayers;
      createFFA.players = 0;
      createFFA.weapons = new string[] { weapon, weapon2 };
      createFFA.map = map;
      createFFA.playersInFFA = new List<FFAPlayer>();
      createFFA.healPerKill = healPerKill;
      createFFA.time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
      ffas.Add(createFFA);
      player.Notify("FFA", $"Du hast eine FFA erstellt mit der ID: {createFFA.id}", 5000);
      JoinFFA(createFFA, player);
    }

    static void RespawnInFFA(xPlayer player)
    {
      FFA ffa = ffas.Find(x => x.playersInFFA.Exists(y => y.playerId == player.id));
      if (ffa == null) return;
      FFAMap map = ffaMaps.Find(x => x.name == ffa.map);
      if (map == null) return;
      if (player.data.stats.isDead)
      {
        player.iPlayer.Spawn(map.spawns[new Random().Next(0, map.spawns.Length - 1)]);
        player.SetDead(false);
      }
      else
      {
        player.iPlayer.Position = map.spawns[new Random().Next(0, map.spawns.Length - 1)];
      }
      player.iPlayer.Armor = 100;
    }

    static void JoinFFA(FFA ffa, xPlayer player)
    {
      FFAPlayer ffap = new FFAPlayer();
      ffap.cachedPosition = player.iPlayer.Position;
      ffap.playerId = player.id;
      ffap.kills = 0;
      ffap.deaths = 0;
      ffa.playersInFFA.Add(ffap);
      player.iPlayer.SendChatMessage($"Du bist der FFA {ffa.id} beigetreten!");

      foreach(Weapon weapon in player.data.loadout)
      {
        player.iPlayer.RemoveWeapon(Alt.Hash(weapon.name));
      }

      foreach (FFAPlayer ffaPlayer in ffa.playersInFFA)
      {
        xPlayer xplayer = Players.getPlayer(ffaPlayer.playerId);
        xplayer.iPlayer.SendChatMessage($"{player.data.name} ist der FFA {ffa.id} beigetreten!");
      }
      foreach (string weapon in ffa.weapons)
      {
        player.iPlayer.GiveWeapon(Alt.Hash(weapon), 9999, true);
      }
      player.iPlayer.Emit("ffa:enter");
      RespawnInFFA(player);
    }

    static void LeaveFFA(FFA ffa, xPlayer player)
    {
      FFAPlayer ffap = ffa.playersInFFA.Find(x => x.playerId == player.id);
      ffa.playersInFFA.Remove(ffap);
      player.iPlayer.SendChatMessage($"Du hast die FFA {ffa.id} verlassen!");
      player.iPlayer.SendChatMessage($"Zussamen fassung:");
      player.iPlayer.SendChatMessage($"Kills: {ffap.kills}");
      player.iPlayer.SendChatMessage($"Deaths: {ffap.deaths}");
      if (ffap.deaths == 0) ffap.deaths = 1;
      if(ffap.kills == 0) ffap.kills = 1;
      float kd = ffap.kills / ffap.deaths;
      kd = (float)Math.Round(kd, 2);
      player.iPlayer.SendChatMessage($"KD: {kd}");

      foreach(string weapon in ffa.weapons)
      {
        player.iPlayer.RemoveWeapon(Alt.Hash(weapon));
      }

      player.Restore();

      foreach (FFAPlayer ffaPlayer in ffa.playersInFFA)
      {
        xPlayer xplayer = Players.getPlayer(ffaPlayer.playerId);
        xplayer.iPlayer.SendChatMessage($"{player.name} hat die FFA {ffa.id} verlassen!");
      }
      player.iPlayer.Emit("ffa:leave");
      player.iPlayer.Position = ffap.cachedPosition;
    }

    [Command("joinffa")]
    public void Joinffa(IPlayer iplayer, int id, string password)
    {
      xPlayer player = Players.getPlayer(iplayer);
      FFA ffa = ffas.Find(x => x.id == id);
      Alt.Log(ffa.name);
      if (ffa == null)
      {
        iplayer.SendChatMessage("Diese FFA existiert nicht!");
        return;
      }
      if (ffa.playersInFFA.Count >= ffa.maxPlayers)
      {
        iplayer.SendChatMessage("Diese FFA ist voll!");
        return;
      }
      if (ffa.password != password)
      {
        iplayer.SendChatMessage("Das Passwort ist falsch!");
        return;
      }
      JoinFFA(ffa, player);
    }

    [Command("leaveffa")]
    public void Leaveffa(IPlayer iplayer)
    {
      xPlayer player = Players.getPlayer(iplayer);
      FFA ffa = ffas.Find(x => x.playersInFFA.Exists(y => y.playerId == player.id));
      if (ffa == null) return;
      LeaveFFA(ffa, player);
    }

    [ScriptEvent(ScriptEventType.PlayerDead)]
    public static async void OnFFADead(IPlayer ipl, IEntity kille, uint weapon)
    {
      xPlayer player = Players.getPlayer(ipl);
      xPlayer killer = Players.getPlayer((IPlayer)kille);
      if(ffas.Find(x => x.playersInFFA.Exists(y => y.playerId == player.id)) == null) return;
      FFA ffa = ffas.Find(x => x.playersInFFA.Exists(y => y.playerId == player.id));

      FFAPlayer ffap = ffa.playersInFFA.Find(x => x.playerId == player.id);
      ffap.deaths++;
      if (killer != null)
      {
        if(ffa.healPerKill) {
          killer.iPlayer.Health = killer.iPlayer.MaxHealth;
          killer.iPlayer.Armor = 100;
        };

        FFAPlayer ffak = ffa.playersInFFA.Find(x => x.playerId == killer.id);
        ffak.kills++;
        foreach (FFAPlayer ffaPlayer in ffa.playersInFFA)
        {
          xPlayer xplayer = Players.getPlayer(ffaPlayer.playerId);
          xplayer.iPlayer.SendChatMessage($"{player.data.name} wurde von {killer.data.name} gekilled!");
        }
      }
      
      await Task.Delay(5000);
      RespawnInFFA(player);
    }
  }
}
