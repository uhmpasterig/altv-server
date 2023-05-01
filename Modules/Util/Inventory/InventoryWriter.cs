using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;

namespace server.Util.Inventory;
public class InventoryWriter : IWritable
{
  private readonly List<xStorage> storages;
  public InventoryWriter(List<xStorage> _storages)
  {
    this.storages = _storages;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("storages");
    writer.BeginArray();
    foreach (xStorage store in storages)
    {
      Console.Write("1");
      writer.BeginObject();

      writer.Name("local_id");
      writer.Value(store.local_id);

      writer.Name("id");
      writer.Value(store.id);
      Console.Write("2");
      writer.Name("name");
      writer.Value(store.name);
      Console.Write("3");
      writer.Name("maxWeight");
      writer.Value(store.maxWeight);
      Console.Write("4");
      writer.Name("currentWeight");
      writer.Value(store.currentWeight);
      Console.Write("5");
      writer.Name("slots");
      writer.Value(store.slots);
      Console.Write("6");
      writer.Name("items");
      writer.BeginArray();
      foreach (Storage_Item item in store.Items)
      {
        Console.WriteLine("Writing item");
        writer.BeginObject();
        writer.Name("id");
        writer.Value(item.id);
        writer.Name("name");
        writer.Value(item.Item_Data.name);
        writer.Name("weight");
        writer.Value(item.Item_Data.weight);
        // TODO add data
        writer.Name("data");
        writer.Value("null");
        // writer.Value($"{item.label} {item.count * item.weight}KG");
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