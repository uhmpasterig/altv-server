import * as alt from 'alt-client';
import { Items } from './Items.js';
import { Frontend } from './Frontend.js';

export class Inventory {
  data = {}
  items = []

  getItems() {
    return this.items;
  }

  getItemBySlot(slot) {
    return this.items.find(item => item.slot === slot);
  }

  updateInventory() {
    let slots = 0;
    let weight = 0;
    this.items.forEach(item => {
      slots += 1;
      weight += item.weight * item.count;
    });
    this.data.slots = slots;
    this.data.weight = weight;
    this.syncUi();
  }

  setInventoryData(inv) {
    this.data = inv;
  }

  getFreeSlot() {
    let slot = 0;
    for (let i = 1; i <= this.data.maxSlots; i++) {
      let item = this.items.find(item => item.slot === i);
      if (!item) {
        slot = i;
        return slot;
      }
    }
    return -1;
  }

  getItemStacks(id, stacksize) {
    return this.items.filter(item => item.id === id && item.count < stacksize);
  }

  syncUi() {
    let inventory = this.data;
    inventory.items = this.items;
    Frontend.emit("inventory:load", JSON.stringify(inventory));
  }

  splitItem(slot, count) {

    slot = Number(slot);
    let item = this.items.find(item => item.slot === slot);
    if (count == null) count = Math.floor(item.count / 2);
    if (count > item.count) return;
    if (count < 1) return;

    let newSlot = this.getFreeSlot();
    if (newSlot === -1) {
      console.log('No free slot');
      return;
    };

    item.count -= count;
    const itemInfo = Items.getItemFromId(item.id);
    item.name = itemInfo.name;
    item.image = itemInfo.image;
    item.weight = itemInfo.weight;

    const newItem = {
      id: item.id,
      name: item.name,
      image: item.image,
      count: count,
      weight: item.weight,
      slot: newSlot
    }
    this.items.push(newItem);
    this.updateInventory();
  }

  removeItem(slot, count) {
    slot = Number(slot);
    if (count != null) {
      let item = this.items.findIndex(item => item.slot === slot);
      this.items[item].count -= count;
      if (this.items[item].count <= 0) {
        this.items.splice(item, 1);
      }
    } else {
      this.items.splice(this.items.indexOf(this.items.find(item => item.slot === slot)), 1);
    }
    this.updateInventory();
  }

  dragItem(slot, newSlot) {
    slot = Number(slot);
    newSlot = Number(newSlot);
    let item = this.items.findIndex(itemm => Number(itemm.slot) === slot);
    let item2 = this.items.findIndex(itemm => Number(itemm.slot) === newSlot);

    if (item == -1 || item2 == -1) {
      if (item != -1) {
        this.items[item].slot = newSlot;
      }
      if (item2 != -1) {
        this.items[item2].slot = slot;
      }
      this.updateInventory();
      return;
    }
    if (this.items[item].id == this.items[item2].id && this.items[item].slot != this.items[item2].slot) {
      let itemInfo = Items.getItemFromId(this.items[item].id);
      if (this.items[item].count + this.items[item2].count <= itemInfo.stacksize) {
        this.items[item2].count += this.items[item].count;
        this.items.splice(item, 1);
      } else {
        let count = this.items[item].count + this.items[item2].count - itemInfo.stacksize;
        this.items[item2].count = itemInfo.stacksize;
        this.items[item].count = count;
      }
    } else {
      this.items[item].slot = newSlot;
      this.items[item2].slot = slot;
    }
    this.updateInventory();
  }

  addItem(item, ignoreMerge) {
    let itemInfo = Items.getItemFromId(item.id);
    if (!itemInfo) {
      console.log('Could not find item in items.js');
      return;
    };
    if (!ignoreMerge) {
      const itemStacks = this.getItemStacks(item.id, itemInfo.stacksize);

      itemStacks.forEach(itemStack => {
        if (itemStack.count + item.count < itemInfo.stacksize) {
          this.items.splice(this.items.indexOf(itemStack), 1);
          itemStack.count += item.count;
          item.count = 0;
          this.items.push(itemStack);
        } else {
          this.items.splice(this.items.indexOf(itemStack), 1);
          let dif = itemInfo.stacksize - itemStack.count;
          itemStack.count = itemInfo.stacksize;
          item.count -= dif;
          this.items.push(itemStack);
        }
      });
    }


    item.name = itemInfo.name;
    item.image = itemInfo.image;
    item.weight = itemInfo.weight;

    let fullStacks = Math.floor(item.count / itemInfo.stacksize);
    let rest = item.count % itemInfo.stacksize;

    for (let i = 0; i < fullStacks; i++) {
      if (item.slot === -1) {
        item.slot = this.getFreeSlot();
      } else if (this.items.find(item => item.slot === item.slot)) {
        item.slot = this.getFreeSlot();
      }
      if (item.slot === -1) {
        console.log('No free slot');
        return;
      };
      let newItem = {
        id: item.id,
        name: item.name,
        image: item.image,
        count: itemInfo.stacksize,
        weight: item.weight,
        slot: item.slot
      }
      this.items.push(newItem);
    }

    if (rest > 0) {
      if (item.slot === -1) {
        item.slot = this.getFreeSlot();
      }
      if (item.slot === -1) {
        console.log('No free slot');
        return;
      };
      let newItem = {
        id: item.id,
        name: item.name,
        image: item.image,
        count: rest,
        weight: item.weight,
        slot: item.slot
      }
      this.items.push(newItem);
    }
    this.updateInventory();
    this.syncInventory();
  }
  syncInventory() {
    let inventory = this.data;
    inventory.items = [];
    this.items.forEach(item => {
      const item2 = {
        id: item.id,
        count: item.count,
        slot: item.slot
      }
      inventory.items.push(item2);
    });
    alt.emitServer('inventorySyncInventory', JSON.stringify(inventory));
  }
}

// {"maxWeight":20.0,"weight":1620.0,"maxSlots":10,"slots":162,"items":[]}