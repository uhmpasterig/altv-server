using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using Newtonsoft.Json;

using server.Handlers.Sql;
using MySql.Data.MySqlClient;
using server.Entitys;

using server;
using server.Player;

using server.Items;

namespace server.Scripts
{
  public class Shop : IScript
  {
    [ClientEvent("payShop")]
    public void PayShop(IPlayer iplayer, string warenkorb)
    {
      xPlayer player = Players.getPlayer(iplayer);
      ShopItem[] warenkorbItems = JsonConvert.DeserializeObject<ShopItem[]>(warenkorb);
      foreach (ShopItem item in warenkorbItems)
      {
        if (player.HasCash(item.price * item.count))
        {
          player.RemoveCash(item.price * item.count);
          Item item1 = ItemFunctions.GetItem(item.name);
          player.GiveItem(item1.name, item.count);
        }
        else
        {
          iplayer.SendChatMessage($"Du hast nicht genug Geld dabei fuer x{item.count} {item.name}!");
        }
      }
    }

    public static int[] GetBlip(string type)
    {
      int[] blip = new int[2];
      if (type == "Supermarkt")
      {
        blip[0] = 59;
        blip[1] = 2;
      }
      else if (type == "Baumarkt")
      {
        blip[0] = 478;
        blip[1] = 2;
      }
      return blip;
    }
    internal class ShopData
    {
      public string name;
      public string ped;
      public string type;
      public Position coords;
      public float rotation;
      public int steuerMult;
    }

    internal class ShopItem
    {
      public string name;
      public int price;
      public int count;
      public string type;
      public string image;
    }

    static List<ShopData> Shops = new List<ShopData>();
    static List<ShopItem> ShopItems = new List<ShopItem>();

    public static async void _init()
    {
      MySqlCommand command = Datenbank.Connection.CreateCommand();
      command.CommandText = "SELECT * FROM shop_items";
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        ShopItem shopItem = new ShopItem();
        shopItem.type = reader.GetString("type");
        shopItem.price = reader.GetInt32("price");
        string itemName = reader.GetString("item");
        Item item = ItemFunctions.GetItem(itemName);
        shopItem.image = item.image;
        shopItem.name = item.label;
        ShopItems.Add(shopItem);
      }
      reader.Close();
      command.CommandText = "SELECT * FROM shops";
      reader = command.ExecuteReader();
      while (reader.Read())
      {
        ShopData shopData = new ShopData();
        shopData.name = reader.GetString("name");
        shopData.ped = reader.GetString("ped");
        shopData.type = reader.GetString("type");
        shopData.coords = JsonConvert.DeserializeObject<Position>(reader.GetString("coords"));
        shopData.rotation = reader.GetFloat("rotation");
        shopData.steuerMult = reader.GetInt32("steuerMult");

        string text = $"Um den Shop {shopData.name} zu öffnen.";
        xEntityData entityData = new xEntityData(text, ENTITY_TYPES.HELPTEXT, shopData.coords, 0, 0, 2);
        xEntity entityText = new xEntity(entityData);
        entityText.Add();

        shopData.coords.Z = shopData.coords.Z - 1.0f;
        xEntityData entityData2 = new xEntityData(shopData.ped, ENTITY_TYPES.PED, shopData.coords, 0, 0, 100);
        xEntity entityPed = new xEntity(entityData2);
        entityPed.Add();

        int[] blip = GetBlip(shopData.type);
        ScriptLoader.Blips.Add(new ScriptLoader.BlipData
        {
          position = shopData.coords,
          sprite = blip[0],
          color = blip[1],
          name = shopData.type
        });

        ScriptLoader.Interactions.Add(new ScriptLoader.Interaction
        {
          position = shopData.coords,
          range = 2,
          action = (player) =>
          {
            string shopItems = JsonConvert.SerializeObject(ShopItems.FindAll(x => x.type == shopData.type));
            player.Emit("openShop", shopData.name, shopItems, shopData.steuerMult);
          }
        });

        Shops.Add(shopData);
      }
      reader.Close();
    }
  }
}