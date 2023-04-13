using AltV.Net.Async;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.EntitySync.Events;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync;
using server.Handlers.Entities;
using System.Numerics;

namespace server.Core;


public class xEntity
{
  public xEntity()
  {
    this.data = new Dictionary<string, object>();
    this.entity = null!;
  }
  public IEntity entity { get; set; }
  
  public uint range { get; set; }
  public ENTITY_TYPES entityType { get; set; }
  public int dimension { get; set; }
  public Position position { get; set; }

  public Dictionary<string, object> data { get; set; }
  
  public void CreateEntity()
  {
    this.entity = AltEntitySync.CreateEntity(1, new Vector3(0,0,0), 0, 80);
  }

  public void Destroy()
  {
    AltEntitySync.RemoveEntity(entity);
    this.entity = null!;
  }

  public void Hide()
  {
    AltEntitySync.RemoveEntity(entity);
  }

  public void Show()
  {
    AltEntitySync.AddEntity(entity);
  }

  public void SetSyncedData(string key, object value)
  {
    this.data[key] = value;
    this.entity.SetData(key, value);
  }

  public void RemoveData(string key)
  {
    this.data[key] = null!;
    this.entity.SetData(key, null!);
  }
}

