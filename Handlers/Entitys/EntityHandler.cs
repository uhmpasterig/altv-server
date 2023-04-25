using AltV.Net.EntitySync;

namespace server.Handlers.Entities;
public enum ENTITY_TYPES : ulong
{
  PROP = 0,
  PED = 1,
  VEHICLE = 2,
}

public class EntityHandler 
{
  public static List<IEntity> Entities = new List<IEntity>();
}