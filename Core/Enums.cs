namespace server.Core;

public enum OWNER_TYPES : int
{
  PLAYER,
  FACTION,
  BUSINESS
}

public enum VEHICLE_TYPES : int
{
  PKW,
  LKW,
  PLANE,
  BOAT
}

public enum ENTITY_TYPES : ulong
{
  PROP = 0,
  PED = 1,
  VEHICLE = 2,
  THREED_TEXT = 3
}

public enum BlipColor {
  WHITE = 0,
  RED = 1,
  GREEN = 2,
  BLUE = 3,
  YELLOW = 5,
  LIGHT_RED = 6
}

public enum CLOTH_TYPES : int 
{ 
  mask = 1, 
  torso = 3, 
  leg = 4, 
  bag = 5, 
  shoe = 6, 
  accessories = 7, 
  undershirt = 8, 
  armor = 9, 
  decal = 10, 
  top = 11 
}

enum GARAGE_TYPES
{
  PKW = 1,
  LKW = 2
}

enum GARAGE_SPRITES : int
{
  PKW = 357,
  LKW = 357
}

enum GARAGE_COLORS : int
{
  PKW = 3,
  LKW = 81
}

class GARAGE_NAMES
{
  static Dictionary<string, string> _names = new Dictionary<string, string>()
  {
    { "PKW", "PKW-Garage" },
    { "LKW", "LKW-Garage" }
  };

  public static string GetName(string name)
  {
    return _names[name];
  }
}

enum VEHICLE_SHOP_TYPE : int
{
  LIMOUSINE = 0,
  SPORTWAGEN = 1,
  LKW = 2,
}