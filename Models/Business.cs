using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net;

namespace server.Models;

[Table("businesses")]
[PrimaryKey("id")]
public partial class Business
{
  public Business()
  {
  }
  public int id { get; set; }
  public string name { get; set; }
  
  public List<Player_Society> Members { get; set; }
}