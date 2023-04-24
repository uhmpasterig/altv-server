using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;

namespace server.Util.Fraktionen;
public class FraktionsWriter : IWritable
{
  public readonly Fraktion fraktion;

  public FraktionsWriter(Fraktion _fraktion)
  {
    this.fraktion = _fraktion;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(fraktion.name);
    writer.Value("ugname");
    writer.Value("Hey");
    writer.Value("rank");
    writer.Value("Leader");
    writer.Value("rank_id");
    writer.Value(12);
    writer.Value("uicolor");
    writer.Value("#ffffff");

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
    writer.Value(fraktion.weapon);
    writer.Name("price");
    writer.Value(1000);
    writer.EndObject();

    writer.EndArray();

    writer.Name("perms");
    writer.BeginArray();
    writer.Value("faction.leader");
    writer.EndArray();

    writer.Name("members");
    writer.BeginArray();
    writer.EndArray();

    writer.Name("fights");
    writer.BeginArray();
    writer.EndArray();

    writer.EndObject();
  }
}