using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using AltV.Net;

namespace server.Models;

[Table("player_weapons")]
[PrimaryKey("id")]
public partial class Player_Weapon
{
  public Player_Weapon()
  {
  }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Player Player { get; set; } = new();

  public string name { get; set; }
  public int ammo { get; set; }
  public byte tintIndex { get; set; }

  public uint hash
  {
    get { return Alt.Hash(name); }
  }
}