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
  public string dlcName { get; set; }
  public int price { get; set; }

  public byte palette { get; set; }

  [NotMapped]
  public string CategoryName { get; set; } = "Unknown";

  [NotMapped]
  public uint dlc
  {
    get
    {
      return dlcName == "0" ? 0 : Alt.Hash(dlcName);
    }
  }
}