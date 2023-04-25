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
  public Player_Cloth() { 
  }
  public int id { get; set; }
  
  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }
  
  public string name { get; set; }
}
