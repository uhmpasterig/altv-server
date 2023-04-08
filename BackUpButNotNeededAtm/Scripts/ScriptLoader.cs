using server.Scripts;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net;

namespace server
{
  class ScriptLoader : IScript
  {
    [ClientEvent("Interact:KeyE")]
    public void InteractKeyE(IPlayer player)
    {
      foreach (Interaction interaction in Interactions)
      {
        if (player.Position.Distance(interaction.position) <= interaction.range)
        {
          interaction.action(player);
        }
      }
    }

    public static void LoadAllScripts()
    {
      Garage._init();
      Shop._init();
      ffa._init();
    }

    public class BlipData
    {
      public Position position;
      public int sprite;
      public int color;
      public string name;
    }
    public class Interaction
    {
      public Position position;
      public int range = 3;
      public Action<IPlayer> action;
    }

    public static List<BlipData> Blips = new List<BlipData>();
    public static List<Interaction> Interactions = new List<Interaction>();
    public static void LoadAllBlipsForPlayer(IPlayer player)
    {
      Alt.Log("Blip Loading...");
      Alt.Log("Blip Count: " + Blips.Count);
      foreach (BlipData blip in Blips)
      {
        Alt.Log("Blip: " + blip.name);
        player.Emit("createBlip", blip.position, blip.sprite, blip.color, blip.name);
      }
    }
  }
}