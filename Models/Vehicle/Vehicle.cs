using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("vehicles")]
[PrimaryKey("id")]
public partial class Vehicle
{
  public Vehicle()
  {
  }

  public int id { get; set; }
  public int owner_type { get; set; }
  public int owner_id { get; set; }
  public string model { get; set; }
  public Vehicle_WorldOffset WorldOffset { get; set; }
}
