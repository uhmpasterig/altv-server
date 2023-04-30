namespace server.Enums;

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