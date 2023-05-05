using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;
using AltV.Net.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using server.Contexts;

namespace server.Core;

public partial class xPlayer
{
  public Player_Accounts Accounts { get; set; }

  private async Task _loadAccounts(Player_Accounts Accounts)
  {
    if (Accounts == null)
    {
      this.Accounts = new Player_Accounts().Default;
      return;
    }

    this.Accounts = Accounts;
  }

  public async Task SaveAccounts()
  {
    var ctx = ServerContext.Instance;
    var dbPlayer = await ctx.Players
      .Include(p => p.Accounts)
      .Where(p => p.id == this.id)
      .FirstOrDefaultAsync();

    if (dbPlayer == null)
    {
      Console.WriteLine($"Player {this.name} not found in database");
      return;
    }

    dbPlayer.Accounts = this.Accounts;
    await ctx.SaveChangesAsync();
    await ctx.ClearInstance();
  }

  public async Task GiveCash(int amount)
  {
    this.Accounts.cash += amount;
  }

  public async Task TakeCash(int amount)
  {
    this.Accounts.cash -= amount;
  }

  public async Task<bool> HasCash(int amount)
  {
    return this.Accounts.cash >= amount;
  }

  public async Task GiveBank(int amount)
  {
    this.Accounts.bank += amount;
  }

  public async Task TakeBank(int amount)
  {
    this.Accounts.bank -= amount;
  }

  public async Task<bool> HasBank(int amount)
  {
    return this.Accounts.bank >= amount;
  }

  public async Task AddDebt(int amount)
  {
    this.Accounts.debt += amount;
  }

  public async Task RemoveDebt(int amount)
  {
    this.Accounts.debt -= amount;
  }

  public async Task<bool> HasCreditCard()
  {
    return this.Accounts.creditCard;
  }

  public async Task SetCreditCard(bool value)
  {
    this.Accounts.creditCard = value;
  }

  public async Task GiveSociety(int amount)
  {
    this.Accounts.society += amount;
  }

  public async Task TakeSociety(int amount)
  {
    this.Accounts.society -= amount;
  }

  public async Task TakePaycheck()
  {
    await this.GiveCash(this.Accounts.society);
    this.Accounts.society = 0;
  }

  public async Task<int> GetCash()
  {
    return this.Accounts.cash;
  }

  public async Task<int> GetBank()
  {
    return this.Accounts.bank;
  }

  public async Task<int> GetDebt()
  {
    return this.Accounts.debt;
  }

  public async Task<int> GetSociety()
  {
    return this.Accounts.society;
  }
}