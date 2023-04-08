import * as natives from 'natives';
import * as alt from 'alt-client';
import { Frontend } from '../Frontend.js';

let Texts = [];
let oldTextData = [];

class xText {
  text = "";
  key = "";
  constructor(text) {
    this.text = text;
    this.key = "E";
  }

  _show() {
    Frontend.emit("showHelpText", {
      text: this.text,
      key: this.key
    });
  }

  _hide() {
    Frontend.emit("hideHelpText");
  }
}

alt.onServer("entitySync:create", (entityId, entityType, position, newEntityData) => {
  if(Number(entityType) != 4) return;

  if(!oldTextData[entityId]) oldTextData[entityId] = newEntityData;

  const text = new xText(oldTextData[entityId].model);
  text._show();
  Texts[entityId] = text;
})

alt.onServer("entitySync:remove", (entityId, entityType) => {
  if(Number(entityType) != 4) return;
  Texts[entityId]._hide();
  Texts[entityId] = null;
})