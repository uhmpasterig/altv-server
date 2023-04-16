using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;



public partial class verarbeiter_farming_data
{
  public verarbeiter_farming_data()
  {
  }

  public string name { get; set; }
  public int time { get; set; }
  public string inputitem { get; set; }
  public string outputitem { get; set; }
  public int ratio { get; set; }
  public string _pos { get; set; }
  

  [NotMapped]
  public Position Position
  {
    get
    {
      return JsonConvert.DeserializeObject<Position>(_pos);
    }
    set
    {
      _pos = JsonConvert.SerializeObject(value);
    }
  }
}