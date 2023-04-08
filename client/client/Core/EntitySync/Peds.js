import * as natives from 'natives';
import * as alt from 'alt-client';

let Peds = [];
let oldEntityData = [];

class xPed {
  position = null;
  hash = null;
  ped = null;
  heading = null;

  constructor(position, heading, model) {
    console.log("create ped")
    this.heading = heading;
    this.position = position;
    this.hash = model;
  }

  create() {
    natives.requestModel(Number(this.hash));
    this.ped = natives.createPed(1, Number(this.hash), this.position.x, this.position.y, this.position.z, this.heading, false, false);
    natives.setEntityHeading(this.ped, this.heading);
    natives.freezeEntityPosition(this.ped, true);
    natives.setEntityInvincible(this.ped, true);
    natives.setEntityCollision(this.ped, false, false);
    natives.setBlockingOfNonTemporaryEvents(this.ped, true);
  }
  update() {
    this.remove();
    this.create();
  }
  remove() {
    natives.deletePed(this.ped);
    this.ped = null;
  }
}

alt.onServer("entitySync:create", (entityId, entityType, position, newEntityData) => {
  if (Number(entityType) != 2) return;

  if (!oldEntityData[entityId]) oldEntityData[entityId] = newEntityData;

  const ped = new xPed(position, oldEntityData[entityId].heading, oldEntityData[entityId].hash);
  ped.create();
  Peds[entityId] = ped;
})

alt.onServer("entitySync:remove", (entityId, entityType) => {
  if (Number(entityType) != 2) return;
  Peds[entityId].remove();
  Peds[entityId] = null;
})