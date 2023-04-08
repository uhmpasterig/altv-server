import * as alt from 'alt-client';
import { Inventory } from './Inventory.js';
import { Frontend } from './Frontend.js';
import * as native from 'natives';

let usableItems = [];

class xPlayer {
  constructor() {
    this.inventory = new Inventory();
    this.cash = 0;
  }

  setCash(cash) {
    this.cash = cash;
  }

  registerUsableItem(name, callback) {
    usableItems[name] = callback;
  }

  useItem(name) {
    console.log("Useitem: " + name)
    if (usableItems[name]) usableItems[name]();
  }
}

const Player = new xPlayer();

alt.onServer('inventorySet', (inv2) => {
  const inv = JSON.parse(inv2);
  Player.inventory.setInventoryData(inv);
  inv.items.forEach(item => {
    Player.inventory.addItem(item, true);
  });
});

alt.onServer('inventoryAddItem', (item) => {
  item = JSON.parse(item);
  Player.inventory.addItem(item);
});

alt.onServer('cash', (cash) => {
  Player.setCash(cash);
  Frontend.emit('updateUiMoney', {
    cash: cash,
    bank: 0
  });
});

alt.onServer('dead', async (bool) => {
  console.log("Dead: " + bool)
  if(bool) {
    native.setEntityInvincible(alt.Player.local.scriptID, true);
    
    native.reviveInjuredPed(alt.Player.local.scriptID);
    native.setEntityHealth(alt.Player.local.scriptID, 100, 0);
    native.freezeEntityPosition(alt.Player.local.scriptID, true);

    native.clearPedTasks(alt.Player.local.scriptID);
    native.clearPedTasksImmediately(alt.Player.local.scriptID);

    
    native.requestAnimDict("missarmenian2");
    while (!native.hasAnimDictLoaded("missarmenian2")) await alt.nextTick();
    native.taskPlayAnim(alt.Player.local.scriptID, "missarmenian2", "corpse_search_exit_ped", 8.0, 1.0, -1, 1, 0, false, false, false);
  } else {
    native.setEntityInvincible(alt.Player.local.scriptID, false);
    native.freezeEntityPosition(alt.Player.local.scriptID, false);
    native.clearPedTasks(alt.Player.local.scriptID);
  }
});    

export default Player;