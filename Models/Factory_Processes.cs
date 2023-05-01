using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;

public class Factory_Reciep
{
  public string item { get; set; }
  public int amount { get; set; }
}


[Table("factory_processes")]
[PrimaryKey("id")]
public partial class Factory_Processes
{
  public Factory_Processes()
  {
  }
  public int id { get; set; }
  public string name { get; set; }
  public string label { get; set; }
  public string inputItems { get; set; }
  public string outputItems { get; set; }
  public int ticksPerProcess { get; set; }

  [NotMapped]
  public float weightNeeded
  {
    get
    {
      float weight = 0;
      foreach (Factory_Reciep item in inputItemsList)
      {
        // TODO : FIXX DAS WIEDER
        weight += 20 * item.amount;
      }
      return weight;
    }
  }

  [NotMapped]
  public List<Factory_Reciep> inputItemsList
  {
    get { return JsonConvert.DeserializeObject<List<Factory_Reciep>>(inputItems); }
    set { inputItems = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public List<Factory_Reciep> outputItemsList
  {
    get { return JsonConvert.DeserializeObject<List<Factory_Reciep>>(outputItems); }
    set { outputItems = JsonConvert.SerializeObject(value); }
  }
}