using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;

namespace server.Modules.Clothing;

public enum CLOTH_TYPES : int 
{ 
  mask = 1, 
  torso = 3, 
  leg = 4, 
  bag = 5, 
  shoe = 6, 
  accessories = 7, 
  undershirt = 8, 
  armor = 9, 
  decal = 10, 
  top = 11 
}

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
    Cloth? cloth = allClothes.Where(c => c.id == id).FirstOrDefault();
    cloth.CategoryName = Enum.GetName(typeof(CLOTH_TYPES), cloth.component)!;
    return cloth;
  }
}