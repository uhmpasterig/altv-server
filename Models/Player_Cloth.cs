using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[Table("player_cloth")]
public partial class Player_Cloth
{
  public Player_Cloth()
  {
  }
  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }

  public int mask { get; set; }
  public int torso { get; set; }
  public int leg { get; set; }
  public int bag { get; set; }
  public int shoe { get; set; }
  public int accessories { get; set; }
  public int undershirt { get; set; }
  public int armor { get; set; }
  public int decal { get; set; }
  public int top { get; set; }

  public void SetPiece(string name, int number)
  {
    var property = this.GetType().GetProperty(name);
    if (property != null)
    {
      property.SetValue(this, number);
    }
  }

  public List<int> ToList()
  {
    return new List<int>() { mask, torso, leg, bag, shoe, accessories, undershirt, armor, decal, top };
  }
}
