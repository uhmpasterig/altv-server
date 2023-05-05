using AltV.Net.Data;
using AltV.Net.Enums;
namespace server.Models;


public enum BLIP_DISPLAYS : int
{
  DEFAULT = 2,
  MAP_ONLY = 3,
  MINIMAP_ONLY = 5,
  BOTH_NOTSELECTABLE = 10
}

public enum BLIP_CATEGORIES : int
{
  NO_DISTANCE = 1,
  DISTANCE = 2,
  PLAYER = 7,
  PROPERTY = 10,
  OWNED_PROPERTY = 11,
}

public class Blip
{
  public Blip(string _name, int _sprite, int _color, double _scale, bool _shortRange, Position _position, BLIP_DISPLAYS _display = BLIP_DISPLAYS.DEFAULT, BLIP_CATEGORIES _category = BLIP_CATEGORIES.NO_DISTANCE)
  {
    name = _name;
    sprite = _sprite;
    color = _color;
    scale = _scale;
    shortRange = _shortRange;
    position = _position;
    display = _display;
    category = _category;
  }
  public int id { get; set; }

  /// <summary>
  /// Defines the name of the string on the map (it will be sorted by the names)
  /// </summary>
  public string name { get; set; }

  /// <summary>
  /// See https://docs.fivem.net/docs/game-references/blips/
  /// </summary>
  public int sprite { get; set; }

  /// <summary>
  /// See https://wiki.rage.mp/index.php?title=Blip::color
  /// </summary>
  public int color { get; set; }

  /// <summary>
  /// 0.00 - 1.00 defines the size of the blip
  /// </summary>
  public double scale { get; set; }

  /// <summary>
  /// 0 = No Route, 1 = Short Route, 2 = Long Route
  /// </summary>
  public bool shortRange { get; set; }

  /// <summary>
  ///SEE BLIP_DISPLAYS
  /// </summary>
  public BLIP_DISPLAYS display { get; set; } = BLIP_DISPLAYS.DEFAULT;

  /// <summary>
  /// SEE BLIP_CATEGORIES
  /// </summary>
  public BLIP_CATEGORIES category { get; set; } = BLIP_CATEGORIES.NO_DISTANCE;

  /// <summary>
  /// Position of the blip
  /// </summary>
  public Position position { get; set; }
}