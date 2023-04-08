namespace server.Core;

interface IServer
{
  void Start();
  Task Stop();
}