using AltV.Net;

namespace server.Modules.Weapons;

public class xWeapon {
  public int id { get; set; }
  public string name { get; set; }
  public int ammo { get; set; }

  string job = "none";

  public xWeapon(int id, string name, int ammo) {
    this.id = id;
    this.name = name;
    this.ammo = ammo;
  }

  public xWeapon(int id, string name, int ammo, string job) {
    this.id = id;
    this.name = name;
    this.ammo = ammo;
    this.job = job;
  }

  public xWeapon(){
    name = "";
    job = "";
  }
}