using System.Data;
using _logger = server.Logger.Logger;

namespace server.Handlers.Dimension;

public class DimensionHandler : IDimensionHandler
{
  public static List<int> Dimensions = new List<int>();

  public async Task<string> GetFreeDimension()
  {
    int random = new Random().Next(10000, 999999);
    if (Dimensions.Contains(random))
    {
      return await GetFreeDimension();
    }
    else
    {
      Dimensions.Add(random);
      return random.ToString();
    }
  }

  public void RemoveDimension(int dimension)
  {
    if (Dimensions.Contains(dimension))
    {
      Dimensions.Remove(dimension);
    }
    else
    {
      _logger.Error($"Dimension {dimension} not found!");
    }
  }
  
  public bool IsValidDimension(int dimension)
  {
    if (!Dimensions.Contains(dimension))
    {
      _logger.Error($"Dimension {dimension} not found!");
      return false;
    }
    return true;
  }
}
