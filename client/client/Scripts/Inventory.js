import { Frontend } from "../Core/Frontend.js";
import Player from "../Core/Player.js";
import * as alt from "alt-client";

let debounce = false;
let debounceMs = 300;
alt.on("keyup", (key) => {
  if (key === 73 && !debounce && !alt.isConsoleOpen() && !Frontend.isAnyUiOpen()) {
    debounce = true;
    openInventory()
    setTimeout(() => {
      debounce = false;
    }, debounceMs);
  }
});

let inventory = {}

function openInventory() {
  console.log("opening inventory")
  Frontend.show("inventory");
  Frontend.emit("inventory:show");
}
alt.onServer("inventory:sync", (data) => {
  console.log("syncing inventory")
  inventory = JSON.parse(data);
  console.log(inventory)
  Frontend.emit("inventory:set", inventory);
})
Frontend.on("inventory:switchSlots", (slot1, slot2, s1, s2) => {
  slot1 = parseInt(slot1);
  slot2 = parseInt(slot2);
  alt.emitServer("inventory:switchSlots", slot1, slot2)
})