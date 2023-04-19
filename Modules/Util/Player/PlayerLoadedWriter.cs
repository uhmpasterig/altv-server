using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;

namespace server.Util.Player;
public class PlayerLoadedWriter : IWritable
{
  private readonly xPlayer player;
  public PlayerLoadedWriter(xPlayer _player)
  {
    this.player = _player;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(player.name);
    writer.Name("id");
    writer.Value(player.id);
    writer.Name("maxArmor");
    writer.Value(player.MaxArmor);
    writer.Name("maxHealth");
    writer.Value(player.MaxHealth);
    writer.Name("armor");
    writer.Value(player.Armor);
    writer.Name("health");
    writer.Value(player.Health);

    // time in 0-24
    int hour = DateTime.Now.Hour;
    int minute = DateTime.Now.Minute;
    int second = DateTime.Now.Second;
    writer.Name("hour");
    writer.Value(hour);
    writer.Name("minute");
    writer.Value(minute);
    writer.Name("second");
    writer.Value(second);


    /* writer.BeginArray();
    foreach (var value in _rpPlayer.EquippedClothes)
    {
        writer.BeginObject();
        writer.Name("c");
        writer.Value(value.Value.component);
        writer.Name("d");
        writer.Value(value.Value.drawable);
        writer.Name("t");
        writer.Value(value.Value.texture);
        writer.EndObject();
    }
    writer.EndArray(); */

    writer.EndObject();
  }
}