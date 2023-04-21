using server.Core;
using server.Models;
using AltV.Net.Data;
using AltV.Net.Enums;

namespace server.Handlers.Dimension;

public interface IDimensionHandler
{
  bool IsValidDimension(int dimension);
  void RemoveDimension(int dimension);
  Task<string> GetFreeDimension();
}