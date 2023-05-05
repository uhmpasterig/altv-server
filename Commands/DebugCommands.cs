using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using AltV.Net;

using server.Models;
using Newtonsoft.Json;

namespace server.Commands;

internal class WeaponCommands : IScript
{

  [Command("setdim")]
  public async static void SetDimension(xPlayer player, int id)
  {
    await player.SetDimension(id);
  }

  [Command("save")]
  public async static void SaveMe(xPlayer player)
  {
    await player.Save();
  }
}