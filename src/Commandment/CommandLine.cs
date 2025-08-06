using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Commandment;

public static class CommandExtensions {
  public static Command AddOpt<T>(this Command self, Option<T> opt) {
    self.Add(opt);
    return self;
  }

  class CustomAsyncAction(Func<ParseResult, CancellationToken, Task<int>> Action) : AsynchronousCommandLineAction {
    public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default) {
      return Action(parseResult, cancellationToken);
    }
  }

  class CustomAction(Func<ParseResult, int> Action) : SynchronousCommandLineAction {
    public override int Invoke(ParseResult parseResult) {
      return Action(parseResult);
    }
  }

  public static Command WithAction(this Command self, Func<ParseResult, int> action) {
    self.Action = new CustomAction(action);
    return self;
  }

  public static Command WithAsyncAction(this Command self, Func<ParseResult, CancellationToken, Task<int>> asyncAction) {
    self.Action = new CustomAsyncAction(asyncAction);
    return self;
  }
}

public static class OptionExtensions {
  public static Option<T> WithArity<T>(this Option<T> self, ArgumentArity arity) {
    self.Arity = arity;
    return self;
  }

  public static Option<T> NoArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.Zero);

  public static Option<T> ZeroOrOneArg<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrOne);

  public static Option<T> ZeroOrMoreArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrMore);

  public static Option<T> OneArg<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ExactlyOne);

  public static Option<T> OneOrMoreArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.OneOrMore);

  public static Option<T> Required<T>(this Option<T> self, bool required) {
    self.Required = required;
    return self;
  }

  public static Option<T> Required<T>(this Option<T> self) => self.Required(true);

  public static Option<T> Optional<T>(this Option<T> self) => self.Required(false);

  public static Option<T> WithParser<T>(this Option<T> self, Func<ArgumentResult, T?> parser) {
    self.CustomParser = parser;
    return self;
  }

  public static Option<T> WithDescription<T>(this Option<T> self, string description) {
    self.Description = description;
    return self;
  }

  public static Option<T> WithDefaultValue<T>(this Option<T> self, T defaultValue) {
    self.DefaultValueFactory = _ => defaultValue;
    return self;
  }

  public static Option<T> WithDefaultFactory<T>(this Option<T> self, Func<ArgumentResult, T> factory) {
    self.DefaultValueFactory = factory;
    return self;
  }

  public static Option<T> WithValidator<T>(this Option<T> self, Action<Option<T>, OptionResult> validator) {
    self.Validators.Add(result => validator(self, result));
    return self;
  }

  public static Option<string> ValidFilePath(this Option<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!File.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  public static Option<string> ValidDirectoryPath(this Option<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!Directory.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  public static Option<string> ValidEnumVariant<E>(this Option<string> self, bool ignoreCase, bool showVariantsOnError)
    where E : struct, Enum
  {
    return self.WithValidator((self, result) => {
      var value = result.GetValue(self);

      if (Enum.TryParse<E>(value, ignoreCase, out var _)) {
        return;
      }

      var sb = new StringBuilder()
        .AppendLine($"'{value}' is not a valid option for argument '{self.Name}'");

      if (showVariantsOnError) {
        IEnumerable<string> validVariants = Enum.GetNames(typeof(E));

        if (ignoreCase) {
          validVariants = validVariants.Select(name => name.ToLower());
        }

        sb.AppendLine($"\tValid options are: [{string.Join(", ", validVariants)}]");
      }

      result.AddError(sb.ToString());
    });
  }
}

