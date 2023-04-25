using Microsoft.EntityFrameworkCore;
using server.Models;
using _logger = server.Logger.Logger;
using Autofac;
using server.Core;
using System.Reflection;


namespace server.Helpers;
internal class Startup : IDisposable
{
  private readonly Type[] _loadedTypes = Assembly.GetExecutingAssembly().GetTypes();

  private IContainer _container;
  private ILifetimeScope _scope;

  private List<Type> _handlerTypes = new List<Type>();
  private List<Type> _moduleTypes = new List<Type>();

  public void Register()
  {
    var builder = new ContainerBuilder();
    var dataAccess = Assembly.GetExecutingAssembly();

    LoadTypes();

    _logger.Startup("Registering Server...");
    builder.RegisterType<Server>()
      .As<IServer>()
      .SingleInstance();

    builder.RegisterAssemblyTypes(dataAccess)
      .Where(t => t.Namespace != null)
      .Where(t => t.Namespace.StartsWith("server.Handlers"))
      .AsImplementedInterfaces();

    foreach (var module in _moduleTypes)
    {
      builder.RegisterType(module)
        .AsSelf()
        .AsImplementedInterfaces()
        .SingleInstance();
    }

    _logger.Startup("Registering Database Context...");
    var optionsBuilder = new DbContextOptionsBuilder<ServerContext>()
        .UseMySql("server=45.157.233.24;database=server;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
            new MySqlServerVersion(new Version(8, 0, 25)));

    builder.RegisterType<ServerContext>()
      .WithParameter("options", optionsBuilder.Options)
      .InstancePerMatchingLifetimeScope();

    _container = builder.Build();
  }

  internal void LoadTypes()
  {
    foreach(Type _type in _loadedTypes)
    {
      if(_type.Namespace != null && _type.Namespace.StartsWith("server.Handlers"))
      {
        _handlerTypes.Add(_type);
      }
      else if(_type.Namespace != null && _type.Namespace.StartsWith("server.Modules"))
      {
        _moduleTypes.Add(_type);
      }
    }
  }

  internal void ResolveTypes()
  {
    _scope = _container.BeginLifetimeScope();
    var dataAccess = Assembly.GetExecutingAssembly();
    foreach (var type in dataAccess.GetTypes())
    {
      if(type.Namespace != null && type.Namespace.StartsWith("server.Modules"))
      {
        _scope.Resolve(type);
      }
    }
  }

  public T Resolve<T>()
  {
    return _scope.Resolve<T>();
  }

  public IContainer GetContainer()
  {
    return _container;
  }

  public void Dispose()
  {
    _scope?.Dispose();
    _container?.Dispose();
    GC.SuppressFinalize(this);
  }
}
