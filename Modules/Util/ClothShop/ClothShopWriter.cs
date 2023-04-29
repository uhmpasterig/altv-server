using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;
using server.Modules.Clothing;

namespace server.Util.ClothShop;
public class ClothShopWriter : IWritable
{
  private readonly Cloth_Shop shop;
  public ClothShopWriter(Cloth_Shop _shop)
  {
    this.shop = _shop;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("categorys");
    writer.BeginArray();
    foreach (var cat in shop.Categorys)
    {
      writer.BeginObject();
      writer.Name("value");
      writer.Value(cat.name);
      writer.Name("name");
      writer.Value(cat.label);

      writer.Name("items");
      writer.BeginArray();
      foreach (Cloth_Shop_Cloth cloth in cat.items)
      {
        writer.BeginObject();
        writer.Name("name");
        writer.Value(cloth.name);
        writer.Name("subItems");
        writer.BeginArray();
        foreach (int cloth_id in cloth.cloth_ids)
        {
          Cloth? _cloth = ClothModule.GetCloth(cloth_id);
          if (_cloth == null) continue;
          writer.BeginObject();
          writer.Name("id");
          writer.Value(_cloth.id);
          writer.Name("name");
          writer.Value(_cloth.name);
          writer.Name("price");
          writer.Value(_cloth.price);
          writer.EndObject();
        }
        writer.EndArray();

        writer.EndObject();
      }
      writer.EndArray();
      writer.EndObject();
    }

    writer.EndArray();
    writer.EndObject();
  }
}