using Microsoft.EntityFrameworkCore;
using server.Models;

using Autofac;
using server.Core;
using System.Reflection;

namespace server.Helpers;

public class Startup
{
  List<string> moduleBlacklist = new List<string> {
    "ProcessData",
    "xBlip"
    };

  Type[] loadedTypes = Assembly.GetExecutingAssembly().GetTypes();
  List<Type> handlers = new List<Type>();
  List<Type> modules = new List<Type>();

  public IContainer _container;
  private ILifetimeScope _scope;

  public void Configure()
  {
    LoadTypes();

    var builder = new ContainerBuilder();

    builder.RegisterType<Server>()
      .As<IServer>()
      .SingleInstance();

    foreach (Type handler in handlers)
    {
      builder.RegisterType(handler)
        .AsImplementedInterfaces()
        .SingleInstance();
    }

    foreach (Type module in modules)
    {
      builder.RegisterType(module)
        .AsImplementedInterfaces()
        .AsSelf()
        .SingleInstance();
    }

    var optionsBuilder = new DbContextOptionsBuilder<ServerContext>()
        .UseMySql("server=45.157.233.24;database=server;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
            new MySqlServerVersion(new Version(8, 0, 25)));

    builder.RegisterType<ServerContext>()
      .WithParameter("options", optionsBuilder.Options)
      .AsSelf()
      .InstancePerLifetimeScope();

    _container = builder.Build();
  }

  public void Resolve()
  {
    _scope = _container.BeginLifetimeScope();

    var server = _scope.Resolve<IServer>();
    server.Start();

    foreach (Type module in modules)
    {
      _scope.Resolve(module);
    }
  }

  private void LoadTypes()
  {
    foreach (Type type in loadedTypes)
    {
      if (IsHandler(type))
        handlers.Add(type);
      if (IsModule(type))
      {
        Console.WriteLine(type.Name);
        modules.Add(type);
      }
    }
  }

  private bool IsHandler(Type type)
  {
    if (type.Namespace == null) return false;

    return type.Namespace.StartsWith("server.Handlers")
    &&
    (
      type.Name == "ItemHandler" ||
      !type.Name.StartsWith("I")
    )
    && !type.Name.StartsWith("<");
  }

  private bool IsModule(Type type)
  {
    if (type.Namespace == null) return false;

    bool isBlacklisted = false;
    foreach (string module in moduleBlacklist)
    {
      if (type.Name.Contains(module))
      {
        isBlacklisted = true;
        break;
      }
    }
    if(isBlacklisted) return false;

    return type.Namespace.StartsWith("server.Modules") && !type.Name.StartsWith("<");
  }
}
