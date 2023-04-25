using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;

namespace server.Modules.Clothing;

class ClothModule : ILoadEvent
{
  ServerContext _serverContext = new ServerContext();
  public ClothModule()
  {
  }

  public static List<Cloth> allClothes = new List<Cloth>();

  public async void OnLoad()
  {
    allClothes = _serverContext.Clothes.ToList();
  }

  public static Cloth? GetCloth(int id)
  {
    return allClothes.Where(c => c.id == id).FirstOrDefault();
  }
}