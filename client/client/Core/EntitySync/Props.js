import * as natives from 'natives';
import * as alt from 'alt-client';

let Props = [];
let oldEntityData = [];

class xProp {
  position = null;
  hash = null;
  entity = null;
  heading = null;

  constructor(position,heading, hash) {
    this.heading = heading;
    this.position = position;
    this.hash = hash;
  }

  create() {
    this.entity = natives.createObjectNoOffset(this.hash, this.position.x, this.position.y, this.position.z, false, false, false);
    natives.setEntityHeading(this.entity, this.heading);
    natives.freezeEntityPosition(this.entity, true);
  }
  update(){
    this.remove();
    this.create();
  }
  remove() {
    natives.deleteObject(this.entity);
    this.entity = null;
  }
}

alt.onServer("entitySync:create", (entityId, entityType, position, newEntityData) => {
  if(Number(entityType) != 1) return;

  if(!oldEntityData[entityId]) oldEntityData[entityId] = newEntityData;

  const prop = new xProp(position, oldEntityData[entityId].heading, oldEntityData[entityId].hash);
  prop.create();
  Props[entityId] = prop;
})

alt.onServer("entitySync:remove", (entityId, entityType) => {
  if(Number(entityType) != 1) return;
  Props[entityId].remove();
  Props[entityId] = null;
})