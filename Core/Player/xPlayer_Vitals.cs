using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;
using AltV.Net.Data;
using Newtonsoft.Json;

namespace server.Core;

public partial class xPlayer
{
  public ushort Thirst { get; set; }
  public ushort Hunger { get; set; }
  public ushort Fitness { get; set; }

  public Player_Vitals Vitals
  {
    get
    {
      return new Player_Vitals
      {
        thirst = this.Thirst,
        hunger = this.Hunger,
        fitness = this.Fitness,
        armor = this.Armor,
        health = this.Health,
        maxArmor = this.MaxArmor,
      };
    }
    set
    {
      this.Thirst = value.thirst;
      this.Hunger = value.hunger;
      this.Fitness = value.fitness;
      this.Armor = value.armor;
      this.Health = value.health;
      this.MaxArmor = value.maxArmor;
    }
  }

  private async Task _loadVitals(Models.Player_Vitals Vitals)
  {
    if (Vitals == null)
    {
      this.Vitals = new Player_Vitals().Default;
      return;
    }
    // TODO Death state should ignore this one
    if (this.Vitals.health <= 0) this.Vitals.health = this.MaxHealth;

    this.Vitals = Vitals;
  }
}