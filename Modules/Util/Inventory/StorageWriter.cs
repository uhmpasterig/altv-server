using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;

namespace server.Util.Inventory;
public class StorageWriter : IWritable
{
  private readonly xStorage storage;
  public StorageWriter(xStorage _storage)
  {
    this.storage = _storage;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();

    writer.Name("local_id");
    writer.Value(storage.local_id);

    writer.Name("id");
    writer.Value(storage.id);
    writer.Name("name");
    writer.Value(storage.name);
    writer.Name("maxWeight");
    writer.Value(storage.maxWeight);
    writer.Name("currentWeight");
    writer.Value(storage.currentWeight);
    writer.Name("slots");
    writer.Value(storage.slots);
    writer.Name("items");
    writer.BeginArray();
    foreach (Storage_Item item in storage.Items)
    {
      writer.BeginObject();
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
}