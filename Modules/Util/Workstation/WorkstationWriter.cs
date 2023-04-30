using AltV.Net;
using server.Models;
using server.Modules.Items;
using _logger = server.Logger.Logger;

namespace server.Util.Workstation;
public class WorkStationWriter : IWritable
{
  public List<Factory_Processes> factoryProcesses = new List<Factory_Processes>();
  public int activeProcessId = -1;

  public WorkStationWriter(List<Factory_Processes> _factoryProcesses, int activeProcessId)
  {
    this.factoryProcesses = _factoryProcesses;
    this.activeProcessId = activeProcessId;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();

    writer.Name("processes");

    writer.BeginArray();
    factoryProcesses.ForEach((process) =>
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(process.id);
      writer.Name("name");
      writer.Value(process.name);
      writer.Name("label");
      writer.Value(process.label);
      writer.Name("state");
      writer.Value(process.id == activeProcessId ? true : false);

      writer.Name("inputItems");
      writer.BeginArray();
      process.inputItemsList.ForEach((item) =>
      {
        writer.BeginObject();
        writer.Name("name");
        writer.Value(item.item);
        writer.Name("amount");
        writer.Value(item.amount);
        writer.Name("label");
        // TODO add label
        // writer.Value(Items.GetItemLabel(item.item));
        writer.Value("");
        writer.EndObject();
      });
      writer.EndArray();

      writer.Name("outputItems");
      writer.BeginArray();
      process.outputItemsList.ForEach((item) =>
      {
        writer.BeginObject();
        writer.Name("name");
        writer.Value(item.item);
        writer.Name("amount");
        writer.Value(item.amount);
        writer.Name("label");

        // TODO add label
        // writer.Value(Items.GetItemLabel(item.item));
        writer.Value("");
        
        writer.EndObject();
      });
      writer.EndArray();
      writer.EndObject();
    });
    writer.EndArray();
    writer.EndObject();
  }
}