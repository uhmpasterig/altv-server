using server.Core;


namespace server.Events;
public interface IPressedEEvent
{
  Task<bool> OnKeyPressE(xPlayer player);
}
