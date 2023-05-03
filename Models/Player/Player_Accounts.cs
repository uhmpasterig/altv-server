using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("player_accounts")]
[PrimaryKey("id")]
public partial class Player_Accounts
{
  public Player_Accounts() { }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Player Player { get; set; } = new();

  public int cash { get; set; }
  public int bank { get; set; }
  public int debt { get; set; }
  public int society { get; set; }
  public bool creditCard { get; set; }

  [NotMapped]
  public Player_Accounts Default => new()
  {
    cash = 0,
    bank = 0,
    debt = 0,
    society = 0,
    creditCard = false
  };
}