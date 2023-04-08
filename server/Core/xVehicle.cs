using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;

namespace server.Core;

public class xVehicle : AsyncVehicle, IxVehicle
{
  public xVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
  }

  public int vehicleId { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }

  public new IxVehicle ToAsync(IAsyncContext _) => this;
}

public partial interface IxVehicle : IVehicle, IAsyncConvertible<IxVehicle>
{
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
}