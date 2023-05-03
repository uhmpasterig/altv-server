using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;

namespace server.Core;

public partial class xPlayer
{
  public List<Player_Weapon> Weapons { get; set; }

  public async Task GiveRPWeapon(string name, int ammo = 0)
  {
    this.GiveWeapon(Alt.Hash(name), ammo, false);
    this.Weapons.Add(new Player_Weapon()
    {
      name = name,
      ammo = ammo
    });
  }

  public async Task RemoveRPWeapon(string name)
  {
    this.Weapons.RemoveAll(x => x.name == name);
  }

  public async Task RemoveAllRPWeapons()
  {
    this.RemoveAllWeapons();
    this.Weapons.Clear();
  }

  public async Task RestoreRPWeapons()
  {
    foreach (var weapon in this.Weapons)
    {
      this.GiveWeapon(weapon.hash, weapon.ammo, false);
      this.SetWeaponTintIndex(weapon.hash, weapon.tintIndex);
    }
  }

  private async Task _loadWeapons(List<Player_Weapon> Weapons)
  {
    this.Weapons = Weapons;
    await this.RestoreRPWeapons();
  }
}