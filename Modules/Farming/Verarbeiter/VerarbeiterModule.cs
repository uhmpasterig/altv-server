using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using server.Handlers.Storage;
using Newtonsoft.Json;



namespace server.Modules.Farming.Verarbeiter;


public class VerarbeiterMain : ILoadEvent
{
  private List<sammler_verarbeiter_data> _verarbeiter = new List<sammler_verarbeiter_data>();


  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    


  }

}