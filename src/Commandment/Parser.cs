using System;
using System.CommandLine;
using System.Linq;
using System.Reflection;

namespace Commandment.Parser;

public static class Parser {
  // public static T Parse<T>() {
  //   var cmdType = typeof(T);
  //   var cmdAttr = cmdType
  //     .GetCustomAttributes(false)
  //     .FirstOrDefault(attr => attr.GetType() == typeof(CommandAttribute)) as CommandAttribute;

  //   if (cmdAttr is not { }) {
  //     throw new InvalidOperationException($"'{typeof(T).FullName}' isn't a command");
  //   }

  //   var cmd = new Command(cmdAttr.Name, cmdAttr.Description);

  //   var opts = cmdType
  //     .GetProperties()
  //     .SelectMany(member => {
  //       var optAttr = member
  //         .GetCustomAttributes(false)
  //         .Where(attr => attr.GetType() == typeof(OptionAttribute<>))
  //         .ToList()
  //         .FirstOrDefault() as OptionAttribute<object>;

  //       return optAttr is { } ? ((PropertyInfo, OptionAttribute<object>)[])[(member, optAttr)] : [];
  //     });

  //   foreach (var (info, optAttr) in opts) {
  //     var optT = typeof(Option<>)
  //       .MakeGenericType(info.PropertyType);
  //     var opt = optT
  //       .GetConstructor([typeof(string), typeof(string[])])!
  //       .Invoke(null, [optAttr.Long, optAttr.Short])!;

  //     if (optAttr.Arity is { } arity) {
  //       optT.GetProperty("Arity")!.SetValue(opt, arity);
  //     }

  //     if (optAttr.Parser is { } parser) {
  //       optT.GetProperty("CustomParser")!.SetValue(opt, parser);
  //     }
  //   }
  // }
}
