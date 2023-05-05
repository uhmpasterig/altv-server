using server.Events;
using server.Contexts;
using Microsoft.EntityFrameworkCore;

using server.Handlers.Logger;

namespace server.Handlers.Society.Business;
public class BusinessHandler : IBusinessHandler, ILoadEvent
{
  ILogger _logger;
  public BusinessHandler(ILogger logger)
  {
    _logger = logger;
  }

  public async Task OnLoad()
  {
  }
}