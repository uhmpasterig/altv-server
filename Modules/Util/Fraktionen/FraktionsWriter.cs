using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;
using server.Modules.Fraktionen;
using System.Globalization;

namespace server.Util.Fraktionen;
public class FraktionsWriter : IWritable
{
  public readonly Fraktion fraktion;
  public readonly xPlayer player;

  public FraktionsWriter(Fraktion _fraktion, xPlayer _player)
  {
    this.fraktion = _fraktion;
    this.player = _player;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(fraktion.name);

    
    writer.Name("rank");
    writer.Value(FraktionsModuleMain.GetRankName(fraktion, player.job_rank));
    writer.Name("rank_id");
    writer.Value(player.job_rank);
    writer.Name("uicolor");
    writer.Value(fraktion.uicolor);
    
    writer.Name("ugname");
    writer.Value("Hey");

    writer.Name("drogenInfo");
    writer.BeginObject();
      writer.Name("droge");
      writer.Value(fraktion.droge);
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
      writer.Value(fraktion.weapon);
      writer.Name("label");
      writer.Value(Items.GetItemLabel(fraktion.weapon));
      writer.Name("price");
      writer.Value(1000);
      writer.EndObject();

    writer.EndArray();
    #endregion

    writer.Name("perms");
    writer.BeginArray();
    foreach (string perm in player.job_perm) writer.Value(perm);
    writer.EndArray();

    writer.Name("members");
    writer.BeginArray();
    foreach(Models.Player _player in FraktionsModuleMain.GetFrakMembers(fraktion.name))
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(_player.permaId);
      writer.Name("name");
      writer.Value(_player.name);
      writer.Name("rank");
      writer.Value(FraktionsModuleMain.GetRankName(fraktion, _player.job_rank));
      writer.Name("rank_id");
      writer.Value(_player.job_rank);
      writer.Name("phone");
      writer.Value(_player.phone);
      writer.Name("lastseen");
      writer.Value(_player.lastLogin.ToString("dd.MM.yyyy HH:mm", CultureInfo.CreateSpecificCulture("de-DE")));
      writer.Name("online");
      writer.Value(_player.isOnline);
      writer.Name("frakname");
      writer.Value(player.job.ToLower());
      writer.EndObject();
    }
    writer.EndArray();

    writer.Name("fights");
    writer.BeginArray();
    writer.EndArray();

    writer.Name("info");
    writer.BeginObject();

      writer.Name("member");
      writer.BeginObject();
        writer.Name("leader");
        writer.Value(2);
        writer.Name("lager");
        writer.Value(2);
        writer.Name("bank");
        writer.Value(2);
        writer.Name("online");
        writer.Value(2);
        writer.Name("insgesamt");
        writer.Value(2);
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
        writer.Value(20);
        writer.Name("slots_used");
        writer.Value(10);
        writer.Name("kilos");
        writer.Value(10);
        writer.Name("kilos_used");
        writer.Value(5);
      writer.EndObject();
      
      writer.Name("general");
      writer.BeginObject();
        writer.Name("funk");
        writer.Value(fraktion.funk);
        writer.Name("fight_funk");
        writer.Value(fraktion.fight_funk);
        writer.Name("ug_funk");
        writer.Value(fraktion.ug_funk);
        writer.Name("warns");
        writer.Value(fraktion.warns);
        writer.Name("creationDate");
        writer.Value(fraktion.creationDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.CreateSpecificCulture("de-DE")));
      writer.EndObject();
      
      writer.Name("motd");
      writer.Value(fraktion.motd);
    writer.EndObject();

    writer.EndObject();
  }
}