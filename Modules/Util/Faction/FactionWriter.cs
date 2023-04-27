using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;
using server.Modules.Factions;
using System.Globalization;

namespace server.Util.Factions;
public class FactionWriter : IWritable
{
  public readonly Faction faction;
  public readonly xPlayer player;
  public readonly xStorage storage;

  public FactionWriter(Faction _faction, xPlayer _player, xStorage _storage)
  {
    this.faction = _faction;
    this.player = _player;
    this.storage = _storage;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(faction.name);

    
    writer.Name("rank");
    writer.Value(FactionModule.GetRankName(faction, player.player_society.faction_rank_id));
    writer.Name("rank_id");
    writer.Value(player.player_society.faction_rank_id);
    writer.Name("uicolor");
    writer.Value(faction.uicolor);
    
    writer.Name("ugname");
    writer.Value("Hey");

    writer.Name("drogenInfo");
    writer.BeginObject();
      writer.Name("droge");
      writer.Value(faction.droge);
      writer.Name("einpreis");
      writer.Value(100);
      writer.Name("auspreis");
      writer.Value(200);
      writer.Name("lager");
      writer.Value(100);
      writer.Name("maxlager");
      writer.Value(1000);
    writer.EndObject();
    
    #region Shopitems
    writer.Name("shopitems");
    writer.BeginArray();
      writer.BeginObject();
      writer.Name("name");
      writer.Value("weapon_pistol_mk2");
      writer.Name("label");
      writer.Value("Pistol MK2");
      writer.Name("price");
      writer.Value(3000);
      writer.EndObject();

      writer.BeginObject();
      writer.Name("name");
      writer.Value(faction.weapon);
      writer.Name("label");
      writer.Value(Items.GetItemLabel(faction.weapon));
      writer.Name("price");
      writer.Value(1000);
      writer.EndObject();
    writer.EndArray();
    #endregion

    writer.Name("perms");
    writer.BeginArray();
    foreach (string perm in player.player_society.FactionPerms) writer.Value(perm);
    writer.EndArray();

    #region Members 
    writer.Name("members");
    writer.BeginArray();
    foreach(Models.Player _player in FactionModule.GetFactionMembers(faction.name)) {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(_player.id);
      writer.Name("name");
      writer.Value(_player.name);
      writer.Name("rank");
      writer.Value(FactionModule.GetRankName(faction, _player.player_society.faction_rank_id));
      writer.Name("rank_id");
      writer.Value(_player.player_society.faction_rank_id);
      writer.Name("phone");
      writer.Value(_player.phone);
      writer.Name("lastseen");
      writer.Value(_player.lastLogin.ToString("dd.MM.yyyy HH:mm", CultureInfo.CreateSpecificCulture("de-DE")));
      writer.Name("online");
      writer.Value(_player.isOnline);
      writer.Name("frakname");
      writer.Value(player.player_society.Faction.name);
      Console.WriteLine("Perms: " + _player.player_society.FactionPerms.Count);
      writer.Name("perms");
      writer.BeginArray();
      foreach (string perm in _player.player_society.FactionPerms) writer.Value(perm);
      writer.EndArray();

      writer.EndObject();
    }
    writer.EndArray();
    #endregion

    writer.Name("fights");
    writer.BeginArray();
    writer.EndArray();

    int leaderPerms = 1;
    int storagePerms = faction.Members.Where(x => x.FactionPerms.Contains("faction.storage")).Count();
    int vehiclePerms = faction.Members.Where(x => x.FactionPerms.Contains("faction.vehicle")).Count();
    int bankPerms = faction.Members.Where(x => x.FactionPerms.Contains("faction.bank")).Count(); 
    int online = faction.Members.Where(x => x.Player.isOnline).Count();
    int offline = faction.Members.Where(x => !x.Player.isOnline).Count();

    writer.Name("info");
    writer.BeginObject();
      writer.Name("member");
      writer.BeginObject();
        writer.Name("leader");
        writer.Value(leaderPerms);
        writer.Name("lager");
        writer.Value(storagePerms);
        writer.Name("bank");
        writer.Value(bankPerms);
        writer.Name("vehicle");
        writer.Value(vehiclePerms);
        writer.Name("online");
        writer.Value(online);
        writer.Name("insgesamt");
        writer.Value(online + offline);
      writer.EndObject();

      writer.Name("vehicle");
      writer.BeginObject();
        writer.Name("aus");
        writer.Value(2);
        writer.Name("ein");
        writer.Value(2);
        writer.Name("abgeschleppt");
        writer.Value(2);
        writer.Name("insgesamt");
        writer.Value(6);
      writer.EndObject();

      writer.Name("lager");
      writer.BeginObject();
        writer.Name("slots");
        writer.Value(storage.slots);
        writer.Name("slots_used");
        writer.Value(storage.items.Count);
        writer.Name("kilos");
        writer.Value(storage.maxWeight);
        writer.Name("kilos_used");
        writer.Value(storage.weight);
      writer.EndObject();
      
      writer.Name("general");
      writer.BeginObject();
        writer.Name("funk");
        writer.Value(faction.funk);
        writer.Name("fight_funk");
        writer.Value(faction.fight_funk);
        writer.Name("ug_funk");
        writer.Value(faction.ug_funk);
        writer.Name("warns");
        writer.Value(faction.warns);
        writer.Name("creationDate");
        writer.Value(faction.creationDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.CreateSpecificCulture("de-DE")));
      writer.EndObject();
      
      writer.Name("motd");
      writer.Value(faction.motd);
    writer.EndObject();

    writer.Name("fights");
    writer.BeginArray();
      
      writer.BeginObject();
        writer.Name("image");
        writer.Value("https://amoure.club/factions/lcn.png");
        writer.Name("name");
        writer.Value("Gangwar");
        writer.Name("useable");
        writer.Value(false);
      writer.EndObject();

      writer.BeginObject();
        writer.Name("image");
        writer.Value("https://amoure.club/factions/lcn.png");
        writer.Name("name");
        writer.Value("Bootcamp");
        writer.Name("useable");
        writer.Value(true);
      writer.EndObject();

      writer.BeginObject();
        writer.Name("image");
        writer.Value("https://amoure.club/factions/lcn.png");
        writer.Name("name");
        writer.Value("Raid'n Defend");
        writer.Name("useable");
        writer.Value(false);
      writer.EndObject();
    
    writer.EndArray();

    writer.EndObject();
  }
}