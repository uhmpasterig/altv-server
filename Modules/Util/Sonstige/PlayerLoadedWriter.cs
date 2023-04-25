using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.ModulesGoofy.Blip;
using server.Models;

namespace server.Util.Blip;
public class BlipWriter : IWritable
{
  private readonly xBlip blip;
  public BlipWriter(xBlip _blip)
  {
    this.blip = _blip;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(blip.name);
    writer.Name("sprite");
    writer.Value(blip.sprite);
    writer.Name("color");
    writer.Value(blip.color);
    writer.Name("scale");
    writer.Value(blip.scale);
    writer.Name("position");

    writer.BeginObject();
    writer.Name("x");
    writer.Value(blip.Position.X);
    writer.Name("y");
    writer.Value(blip.Position.Y);
    writer.Name("z");
    writer.Value(blip.Position.Z);
    writer.EndObject();

    writer.EndObject();
  }
}