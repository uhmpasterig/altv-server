using AltV.Net;
using _logger = server.Logger.Logger;

namespace server.Util.Shop;
public class shopWriter : IWritable
{
  public Models.Shop shop { get; set; }

  public shopWriter(Models.Shop _shop)
  {
    this.shop = _shop;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(shop.name);
    writer.Name("type");
    writer.Value(shop.typeString);
    writer.Name("items");
    writer.BeginArray();
    foreach (Models.ShopItems item in shop.items.ToList())
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(item.id);
      writer.Name("name");
      writer.Value(item.label);
      writer.Name("type");
      writer.Value(item.type);
      writer.Name("price");
      writer.Value(item.price);
      writer.EndObject();
    }
    writer.EndArray();
    writer.EndObject();
  }
}