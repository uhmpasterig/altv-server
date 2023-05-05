using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace server.Extensions;
internal static class IEnumerableExtensions
{
  internal static Task ForEach<T>(this IEnumerable<T> elements, Action<T> action)
  {
    foreach (var element in elements)
    {
      action(element);
    }

    return Task.CompletedTask;
  }

  internal async static Task ForEachAsync<T>(this IEnumerable<T> elements, Func<T, Task> action)
  {
    foreach (var element in elements)
    {
      await action(element);
    }
  }
}