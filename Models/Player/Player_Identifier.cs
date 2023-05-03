using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using AltV.Net;

namespace server.Models;

[Table("player_identifiers")]
[PrimaryKey("id")]
public partial class Player_Identifier
{
  public Player_Identifier()
  {
  }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Player Player { get; set; } = new();

  public string launcher_name { get; set; }
  public string _hwid { get; set; }

  [NotMapped]
  public ulong HWID
  {
    get
    {
      if (_hwid == null || _hwid == "")
      {
        return 0;
      }
      return Convert.ToUInt64(_hwid);
    }
    set
    {
      _hwid = value.ToString();
    }
  }
}