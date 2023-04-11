using AltV.Net.Async;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.EntitySync.Events;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync;

namespace server.Handlers.Entities;
public enum ENTITY_TYPES : ulong
{
  PROP = 0,
  PED = 1,
  VEHICLE = 2,
}