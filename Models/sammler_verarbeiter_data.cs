using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;



public partial class sammler_verarbeiter_data
{
  public sammler_verarbeiter_data()
  {
  }

  public string name { get; set; }
  public int time { get; set; }
  public string inputitem { get; set; }
  public string outputitem { get; set; }
  public string ratio { get; set; }
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




  

  [NotMapped]
  public List<xEntity> Entities { get; set; } = new List<xEntity>();

  public override string ToString()
  {
    return $"Name: {name}, Time: {time}, InputItem: {inputitem}, OutputItem: {outputitem}, Ratio: {ratio}, Position: {Position}";
  }
}