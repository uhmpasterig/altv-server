using Microsoft.EntityFrameworkCore;
using server.Models;
using _logger = server.Logger.Logger;
using Autofac;
using server.Core;
using System.Reflection;
namespace server.Helpers;

public static class Startup
{
  public static IContainer Configure()
  {
    var builder = new ContainerBuilder();

    builder.RegisterType<Server>()
      .As<IServer>()
      .SingleInstance();

    builder.RegisterAssemblyTypes(Assembly.Load(nameof(server)))
      .Where(t => t.Namespace.Contains("Handlers"))
      .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name))
      .SingleInstance();

    builder.RegisterAssemblyTypes(Assembly.Load(nameof(server)))
      .Where(t => t.Namespace.Contains("Modules"))
      .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name))
      .AsSelf()
      .AsImplementedInterfaces()
      .SingleInstance();

    _logger.Startup("Registering Database Context...");
    var optionsBuilder = new DbContextOptionsBuilder<ServerContext>()
        .UseMySql("server=45.157.233.24;database=server;user=root;password=KrjganovOnTop1!23;treattinyasboolean=true",
            new MySqlServerVersion(new Version(8, 0, 25)));

    builder.RegisterType<ServerContext>()
      .WithParameter("options", optionsBuilder.Options)
      .AsSelf()
      .InstancePerLifetimeScope();

    return builder.Build();
  }
}
