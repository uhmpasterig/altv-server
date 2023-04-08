// using System;
// using AltV.Net.Elements.Entities;
// using AltV.Net;
// using AltV.Net.Resources.Chat.Api;
// using server.Player;
// using server;
// using server.System.Save;


// namespace server.Commands
// {
//   public class Admincommands : IScript
//   {
//     [Command("ban")]
//     public void Ban(IPlayer iplayer, int id, string reason = "Kein Grund Angegeben!")
//     {
//       xPlayer player = Players.getPlayer(id);
//       if (player == null)
//       {
//         iplayer.SendChatMessage("Dieser Spieler Exsistiert nicht!");
//         return;
//       }
//       player.Ban(reason);
//       iplayer.SendChatMessage($"Du hast {player.name} Permanent gebannt!");
//     }

//     [Command("tempban")]
//     public void Tempban(IPlayer iplayer, int id,  int time = 1, string dayweekmonth = "month", string reason = "Kein Grund Angegeben!")
//     {
//       xPlayer player = Players.getPlayer(id);
//       if (player == null)
//       {
//         iplayer.SendChatMessage("Dieser Spieler Exsistiert nicht!");
//         return;
//       }
//       player.Tempban(time, dayweekmonth, reason);
//       iplayer.SendChatMessage($"Du hast {player.name} für {time} {dayweekmonth} gebannt!");
//     }

//     [Command("discordid")]
//     public void Discordid(IPlayer iplayer)
//     {
//       xPlayer player = Players.getPlayer(iplayer);
//       iplayer.SendChatMessage($"Discord ID von {iplayer.Name}: {iplayer.DiscordId.ToString()}");
//     }

//     [Command("unban")]
//     public void Unban(IPlayer iplayer, int unbanid = 0, string discordid = "0")
//     {
//       if (unbanid == 0 && discordid == "0")
//       {
//         iplayer.SendChatMessage("Du musst eine ID oder eine Discord ID angeben!");
//         return;
//       }
//       if (unbanid != 0)
//       {
//         Handlers.Ban.Ban.Unban(unbanid: unbanid);
//       }
//       else if (discordid != "0")
//       {
//         Handlers.Ban.Ban.Unban(discordid: discordid);
//       }
      
//     }
    
//   }
// }