using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net;

namespace server.Models;

[Table("clothes")]
[PrimaryKey("id")]
public partial class Cloth
{
  public Cloth()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public byte component { get; set; }
  public ushort drawable { get; set; }
  public byte texture { get; set; }
  public byte palette { get; set; } = 0;
  public string dlcName { get; set; }

  [NotMapped]
  public uint dlc
  {
    get
    {
      return dlcName == "0" ? 0 : Alt.Hash(dlcName);
    }
  }
}