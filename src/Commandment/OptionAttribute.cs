using System;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Commandment;

interface IOptionAttribute {
  public string? Long { get; }
  public string? Short { get; }
  public ArgumentArity? Arity { get; }
  public bool Required { get; }
}

[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute<T> : Attribute, IOptionAttribute {
  public string? Long { get; set; } = null;
  public string? Short { get; set; } = null;
  public Action<Option<T>, ArgumentResult>? Parser { get; set; } = null;
  public Action<Option<T>, ArgumentResult>? Validator { get; set; } = null;
  public ArgumentArity? Arity { get; set; } = null;
  public bool Required { get; set; } = false;
}
