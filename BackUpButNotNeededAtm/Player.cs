using System;
using AltV.Net.Elements.Entities;
using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using server.Player;
using server;
using server.System.Save;


namespace server.Commands
{
  public class Player : IScript
  {
    [Command("money")]
    public void Money(IPlayer iplayer, string addremove = "", string handorbank = "", int amount = 0)
    {
      xPlayer player = Players.getPlayer(iplayer);
      if (addremove == "" || handorbank == "" || amount == 0)
      {
        iplayer.SendChatMessage($"Du hast {player.cash}$ in der Hand und {player.bank}$ auf dem Konto");
        return;
      }
      
      if (addremove == "add")
      {
        if (handorbank == "hand")
        {
          player.AddCash(amount);
          iplayer.SendChatMessage($"Du hast {amount}$ zur Hand Hinzugefügt");
          
        }
        if (handorbank == "bank")
        {
          player.AddBank(amount);
          iplayer.SendChatMessage($"Du hast {amount}$ auf dein Konto bekommen");
          
        }
        if (handorbank == "")
        {
          iplayer.SendChatMessage("Syntax: /money <add/remove> <hand/bank> <amount>");
        }
      }
      if (addremove == "remove")
      {
        if (handorbank == "hand")
        {
          player.RemoveCash(amount);
          iplayer.SendChatMessage($"Du hast {amount}$ aus der Hand entfernt");
          
        }
        if (handorbank == "bank")
        {
          player.RemoveBank(amount);
          iplayer.SendChatMessage($"Du hast {amount}$ von deinem Konto entfernt");
          
        }
        if (handorbank == "")
        {
          iplayer.SendChatMessage("Syntax: /money <add/remove> <hand/bank> <amount>");
        }
      }
    
    }
  }
}