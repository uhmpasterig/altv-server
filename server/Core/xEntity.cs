using AltV.Net.Async;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;

namespace server.Core;

public enum ENTITY_TYPES 
{
  PROP = 0,
  PED = 1,
  VEHICLE = 2,
}

public interface IxEntity
{
  int id { get; set; }

  string name { get; set; }
  ENTITY_TYPES entityType { get; set; }
  
  int dimension { get; set; }
  Position position { get; set; }

  void SetData(string key, object value);
}

public class xProp : IxEntity
{
  public int id { get; set; }
  public string name { get; set; }
  public ENTITY_TYPES entityType { get; set; }
  
  public int dimension { get; set; }
  public Position position { get; set; }

  public void SetData(string key, object value)
  {
    // AltAsync.Do(() => AltAsync.CreateProp(0, position, new Rotation(0, 0, 0)).SetData(key, value));
  }
}
