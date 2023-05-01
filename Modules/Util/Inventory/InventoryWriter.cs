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
      new StorageWriter(store).OnWrite(writer);
    }
    writer.EndArray();
    writer.EndObject();
  }
}