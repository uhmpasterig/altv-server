using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using server.Handlers.Entities;
using System.Numerics;

namespace server.Core;


public class xEntity
{
  public xEntity()
  {
    this.data = new Dictionary<string, object>();
  }
  
  public uint range { get; set; }
  public ENTITY_TYPES entityType { get; set; }
  public int dimension { get; set; }
  public Position position { get; set; }

  public Dictionary<string, object> data { get; set; }
  
  public void CreateEntity()
  {
  }

  public void Destroy()
  {
  }

  public void Hide()
  {
  }

  public void Show()
  {
  }

  public void SetSyncedData(string key, object value)
  {
    this.data[key] = value;
  }

  public void RemoveData(string key)
  {
    this.data[key] = null!;
  }
}

