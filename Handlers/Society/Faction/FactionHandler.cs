using server.Events;
using server.Contexts;
using Microsoft.EntityFrameworkCore;

using server.Handlers.Logger;

namespace server.Handlers.Faction;
public class FactionHandler : IFactionHandler, ILoadEvent
{
  ILogger _logger;
  public FactionHandler(ILogger logger)
  {
    _logger = logger;
  }

  public IEnumerable<Models.Faction> Factions { get; set; }

  public async Task OnLoad()
  {
    ServerContext ctx = ServerContext.Instance;
    _logger.Debug("Loading factions");

    Factions = await ctx.Factions
      .Include(f => f.Ranks)
      .ToListAsync();

    await ctx.ClearInstance();

    _logger.Debug($"x{Factions.Count()} Faction/s loaded");
  }
}