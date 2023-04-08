import * as alt from 'alt-client';
import * as natives from 'natives';

console.log("server create blip")
alt.onServer("createBlip", (position, sprite, color, name) => {
  console.log("server create blip222")
  const blip = natives.addBlipForCoord(position.x, position.y, position.z);
  natives.setBlipSprite(blip, sprite);
  natives.setBlipColour(blip, color);
  natives.setBlipScale(blip, 0.75);
  natives.setBlipAsShortRange(blip, true);
  natives.beginTextCommandSetBlipName("STRING");
  natives.addTextComponentSubstringPlayerName(name);
  natives.endTextCommandSetBlipName(blip);
});