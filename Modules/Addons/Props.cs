using server.Events;
using server.Core;
using server.Models;
using AltV.Net.Data;
using AltV.Net.Async;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;

using server.Handlers.Player;
using server.Handlers.Event;
using server.Handlers.Storage;
using server.Handlers.Entities;


namespace server.Modules.Props;

public class Props : ILoadEvent
{
  public Props()
  {
  }
  
  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    foreach (Models.Prop _prop in serverContext.Props) {
      CreateProp(_prop);
    }

    AltAsync.OnClient<xPlayer, string, string, string>("createprop", async (player, prop, _position, _rotation) =>
    {
      Position pos = JsonConvert.DeserializeObject<Position>(_position);
      Rotation rot = JsonConvert.DeserializeObject<Rotation>(_rotation);
      CreateDbProp(pos, rot, prop);
    });
  }

  public async static void CreateProp(Models.Prop _prop)
  {
    xEntity _entity = new xEntity();
    _entity.entityType = ENTITY_TYPES.PROP; 
    _entity.dimension = (int)DIMENSIONEN.WORLD;
    _entity.position = _prop.Position;
    _entity.range = 250;
    _entity.data.Add("model", _prop.prop);

    _entity.data.Add("rotation", _prop._rot);
    _entity.CreateEntity();
  }

  public async static void CreateDbProp(Position pos, Rotation rot, string prop)
  {
    await using ServerContext serverContext = new ServerContext();
    Models.Prop _prop = new Models.Prop();
    _prop.Position = pos;
    _prop.Rotation = rot;
    _prop.prop = prop;
    serverContext.Props.Add(_prop);
    await serverContext.SaveChangesAsync();
    CreateProp(_prop);
  }
}