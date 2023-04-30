using Microsoft.EntityFrameworkCore;
using server.Models;

using Autofac;
using server.Core;
using System.Reflection;
namespace server.Helpers;

public static class Startup
{
  static Type[] loadedTypes = Assembly.GetExecutingAssembly().GetTypes();
  static List<Type> handlers = new List<Type>();
  static List<Type> modules = new List<Type>();

  public static IContainer Configure()
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

    return builder.Build();
  }

  private static void LoadTypes()
  {
    foreach (Type type in loadedTypes)
    {
      if (IsHandler(type))
        handlers.Add(type);
      if (IsModule(type))
        modules.Add(type);
    }
  }

  private static bool IsHandler(Type type)
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

  private static bool IsModule(Type type)
  {
    if (type.Namespace == null) return false;

    return type.Namespace.StartsWith("server.Modules") && !type.Name.StartsWith("<"); ;
  }
}
