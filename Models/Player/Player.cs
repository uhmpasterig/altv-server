using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[Table("players")]
[PrimaryKey("id")]
public partial class Player
{
  public Player()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public uint ped { get; set; }

  public List<Player_Weapon> Weapons { get; set; } = new();
  public Player_WorldOffset WorldOffset { get; set; }
  public Player_Identifier Identifier { get; set; }
  public Player_Vitals Vitals { get; set; }
  public Player_Accounts Accounts { get; set; }
}
