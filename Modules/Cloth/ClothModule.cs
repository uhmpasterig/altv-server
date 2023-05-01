using server.Core;
using server.Events;
using server.Models;

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

  public static string GetCategoryName(int componentId)
  {
    return ((CLOTH_TYPES)componentId).ToString();
  }

  public static Cloth? GetCloth(int id)
  {
    Cloth? cloth = allClothes.Where(c => c.id == id).FirstOrDefault();
    return cloth;
  }
}