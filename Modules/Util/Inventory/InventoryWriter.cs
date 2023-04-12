using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;

namespace server.Util.Inventory;
public class inventoryWriter : IWritable
{
  private readonly List<xStorage> storages;
  public inventoryWriter(List<xStorage> _storages)
  {
    this.storages = _storages;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("storages");
    writer.BeginArray();
    foreach(xStorage store in storages) 
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(store.id);
      writer.Name("name");
      writer.Value(store.name);
      writer.Name("maxWeight");
      writer.Value(store.maxWeight);
      writer.Name("currentWeight");
      writer.Value(store.currentWeight);
      writer.Name("slots");
      writer.BeginArray();
      foreach(InventoryItem item in store.items)
      {
        writer.BeginObject();
        writer.Name("id");
        writer.Value(item.id);
        writer.Name("name");
        writer.Value(item.name);
        writer.Name("weight");
        writer.Value(item.weight);
        writer.Name("data");
        writer.Value("Heyy");
        writer.Name("image");
        writer.Value(item.image);        
        writer.Name("count");
        writer.Value(item.count);
        writer.Name("slot");
        writer.Value(item.slot);
        writer.EndObject();
      }      
      writer.EndArray();
      writer.EndObject();
    }
    writer.EndArray();
    writer.EndObject();
  }
}