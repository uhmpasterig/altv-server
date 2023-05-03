using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("player_vitals")]
[PrimaryKey("id")]
public partial class Player_Vitals
{
  public Player_Vitals() { }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Player Player { get; set; } = new();

  public ushort health { get; set; }
  public ushort armor { get; set; }
  public ushort maxArmor { get; set; }

  public ushort hunger { get; set; }
  public ushort thirst { get; set; }
  public ushort fitness { get; set; }

  public bool isDead { get; set; }
  public string? deathCause { get; set; }

  // set function of all vitals
  public async Task SaveAsync(Player_Vitals Player_Vitals)
  {
    this.health = Player_Vitals.health;
    this.armor = Player_Vitals.armor;
    this.maxArmor = Player_Vitals.maxArmor;
    this.hunger = Player_Vitals.hunger;
    this.thirst = Player_Vitals.thirst;
    this.fitness = Player_Vitals.fitness;

    this.isDead = Player_Vitals.isDead;
    this.deathCause = Player_Vitals.deathCause;
  }

  [NotMapped]
  public Player_Vitals Default => new()
  {
    health = 200,
    armor = 0,
    maxArmor = 100,
    hunger = 100,
    thirst = 100,
    fitness = 100,
    isDead = false,
    deathCause = null
  };
}
