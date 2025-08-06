using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Commandment.Parser;

public class CommandAttribute : Attribute {
  public string Name { get; set; } = GetExecutableName();
  public string? Description { get; set; } = null;

  private static string GetExecutableName() {
    if (Assembly.GetEntryAssembly()?.Location is { } exePath) {
      return Path.GetFileName(exePath);
    } else {
      return Process.GetCurrentProcess().ProcessName;
    }
  }
}
