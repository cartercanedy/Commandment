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
  [Obsolete($"Use {nameof(AddOption)} instead")]
  public static Command AddOpt<T>(this Command self, Option<T> opt) => self.AddOption(opt);


  /// <summary>Adds an option to this <see cref="Command"/> instance</summary>
  /// <returns>This <see cref="Command"/> instance for additional call chaining.</returns>
  public static Command AddOption<T>(this Command self, Option<T> opt) {
    self.Add(opt);
    return self;
  }

  /// <summary>Adds a positional argument to this <see cref="Command"/> instance</summary>
  /// <returns>This <see cref="Command"/> instance for additional call chaining.</returns>
  public static Command AddArgument<T>(this Command self, Argument arg) {
    self.Add(arg);
    return self;
  }

  /// <summary>
  /// Adds <paramref name="command"/> to the list of subcommands of this <see cref="Command"/> instance
  /// </summary>
  /// <returns>This <see cref="Command"/> instance for additional call chaining.</returns>
  public static Command AddSubcommand<T>(this Command self, Command command) {
    self.Add(command);
    return self;
  }

  private class CustomAsyncAction(Func<ParseResult, CancellationToken, Task<int>> Action) : AsynchronousCommandLineAction {
    public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default) =>
      Action(parseResult, cancellationToken);
  }

  private class CustomAction(Func<ParseResult, int> Action) : SynchronousCommandLineAction {
    public override int Invoke(ParseResult parseResult) => Action(parseResult);
  }

  /// <summary>
  /// The action to perform upon successful parsing and invokation of the <see cref="Command"/>
  /// The <paramref name="asyncAction"/> will be run asynchronously
  /// </summary>
  /// <param name="self"></param>
  /// <param name="asyncAction"></param>
  /// <returns></returns>
  public static Command WithAction(this Command self, Func<ParseResult, int> action) {
    self.Action = new CustomAction(action);
    return self;
  }

  /// <summary>
  /// The action to perform upon successful parsing and invokation of the <see cref="Command"/>
  /// The <paramref name="asyncAction"/> will be invoked asynchronously
  /// </summary>
  /// <param name="self"></param>
  /// <param name="asyncAction"></param>
  /// <returns></returns>
  public static Command WithAsyncAction(this Command self, Func<ParseResult, CancellationToken, Task<int>> asyncAction) {
    self.Action = new CustomAsyncAction(asyncAction);
    return self;
  }
}

public static class ArgumentExtensions {
  /// <summary>
  /// Specifies the arity (number of accepted values) that can be supplied to this <see cref="Argument{T}"/> instance
  /// </summary>
  /// <param name="self"></param>
  /// <param name="arity"></param>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithArity<T>(this Argument<T> self, ArgumentArity arity) {
    self.Arity = arity;
    return self;
  }

  /// <summary>This argument will accept no values (i.e. it will be nullary)</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> NoArgs<T>(this Argument<T> self) =>
    self.WithArity(ArgumentArity.Zero);

  /// <summary>This argument accepts either 0 or 1 value</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> ZeroOrOneArg<T>(this Argument<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrOne);

  /// <summary>This argument will accept any number of values (including zero)</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> ZeroOrMoreArgs<T>(this Argument<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrMore);

  /// <summary>This argument will accept only one value (i.e. it will be unary)</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> OneArg<T>(this Argument<T> self) =>
    self.WithArity(ArgumentArity.ExactlyOne);

  /// <summary>This argument will accept only one or more values</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> OneOrMoreArgs<T>(this Argument<T> self) =>
    self.WithArity(ArgumentArity.OneOrMore);

  /// <summary>
  /// <para>Provides a delegate to be invoked after the intial token stream parse.</para>
  /// <para>The <paramref name="parser"/> will be invoked to resolve the actual value to assign to this <see cref="Argument{T}"/>.</para>
  /// </summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithParser<T>(this Argument<T> self, Func<ArgumentResult, T?> parser) {
    self.CustomParser = parser;
    return self;
  }

  /// <summary>Sets a description that will be shown in the <c>-h/--help</c> spew</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithDescription<T>(this Argument<T> self, string description) {
    self.Description = description;
    return self;
  }

  /// <summary>Sets a default value that may be used if no value is specified</summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithDefaultValue<T>(this Argument<T> self, T defaultValue) {
    self.DefaultValueFactory = _ => defaultValue;
    return self;
  }

  /// <summary>
  /// Specifies a factory method that will be used to lazily evaluate a default value if no value is provided to this <see cref="Argument{T}"/>
  /// </summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithDefaultFactory<T>(this Argument<T> self, Func<ArgumentResult, T> factory) {
    self.DefaultValueFactory = factory;
    return self;
  }

  /// <summary>
  /// <para>Provides a delegate to be invoked after the a value has been resolved for this <see cref="Argument{T}"/>.</para>
  /// <para>The <paramref name="validator"/> will be invoked to perform argument validation.</para>
  /// <para>May be specified multiple times, with each validator being invoked in an unspecified order.</para>
  /// </summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<T> WithValidator<T>(this Argument<T> self, Action<Argument<T>, ArgumentResult> validator) {
    self.Validators.Add(result => validator(self, result));
    return self;
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Argument{T}"/> is a valid path to a file
  /// </summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<string> ValidFilePath(this Argument<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!File.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Argument{T}"/> is a valid path to a directory
  /// </summary>
  /// <returns>This <see cref="Argument{T}"/> instance for additional call chaining</returns>
  public static Argument<string> ValidDirectoryPath(this Argument<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!Directory.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Argument{E}"/> is a valid string representation of the enumeration variant of <typeparamref name="E"/>
  /// </summary>
  /// <returns>This <see cref="Argument{E}"/> instance for additional call chaining</returns>
  public static Argument<string> ValidEnumVariant<E>(this Argument<string> self, bool ignoreCase, bool showVariantsOnError)
    where E : struct, Enum
  {
    return self.WithValidator((self, result) => {
      var value = result.GetValue(self);

      if (Enum.TryParse<E>(value, ignoreCase, out var _)) {
        return;
      }

      var sb = new StringBuilder()
        .AppendLine($"'{value}' is not a valid value for argument '{self.Name}'");

      if (showVariantsOnError) {
        IEnumerable<string> validVariants = Enum.GetNames(typeof(E));

        if (ignoreCase) {
          validVariants = validVariants.Select(name => name.ToLower());
        }

        sb.AppendLine($"\tValid arguments are: [{string.Join(", ", validVariants)}]");
      }

      result.AddError(sb.ToString());
    });
  }
}
public static class OptionExtensions {
  /// <summary>
  /// Specifies the arity (number of accepted values) that can be supplied to this <see cref="Option{T}"/> instance
  /// </summary>
  /// <param name="self"></param>
  /// <param name="arity"></param>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithArity<T>(this Option<T> self, ArgumentArity arity) {
    self.Arity = arity;
    return self;
  }

  /// <summary>This option will accept no values (i.e. it will be nullary)</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> NoArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.Zero);

  /// <summary>This option accepts either 0 or 1 value</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> ZeroOrOneArg<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrOne);

  /// <summary>This option will accept any number of values (including zero)</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> ZeroOrMoreArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ZeroOrMore);

  /// <summary>This option will accept only one value (i.e. it will be unary)</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> OneArg<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.ExactlyOne);

  /// <summary>This option will accept only one or more values</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> OneOrMoreArgs<T>(this Option<T> self) =>
    self.WithArity(ArgumentArity.OneOrMore);

  /// <summary>
  /// <para>Marks this option as required if <paramref name="required"/> is <see langword="true"/>.</para>
  /// <para>Otherwise, this option will be marked as optional</para>
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> Required<T>(this Option<T> self, bool required) {
    self.Required = required;
    return self;
  }

  /// <summary>Marks this option as required</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> Required<T>(this Option<T> self) => self.Required(true);

  /// <summary>Marks this option as not required</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> Optional<T>(this Option<T> self) => self.Required(false);

  /// <summary>
  /// <para>Provides a delegate to be invoked after the intial token stream parse.</para>
  /// <para>The <paramref name="parser"/> will be invoked to resolve the actual value to assign to this <see cref="Option{T}"/>.</para>
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithParser<T>(this Option<T> self, Func<ArgumentResult, T?> parser) {
    self.CustomParser = parser;
    return self;
  }

  /// <summary>Sets a description that will be shown in the <c>-h/--help</c> spew</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithDescription<T>(this Option<T> self, string description) {
    self.Description = description;
    return self;
  }

  /// <summary>Sets a default value that may be used if no value is specified</summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithDefaultValue<T>(this Option<T> self, T defaultValue) {
    self.DefaultValueFactory = _ => defaultValue;
    return self;
  }

  /// <summary>
  /// Specifies a factory method that will be used to lazily evaluate a default value if no value is provided to this <see cref="Option{T}"/>
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithDefaultFactory<T>(this Option<T> self, Func<ArgumentResult, T> factory) {
    self.DefaultValueFactory = factory;
    return self;
  }

  /// <summary>
  /// <para>Provides a delegate to be invoked after the a value has been resolved for this <see cref="Option{T}"/>.</para>
  /// <para>The <paramref name="validator"/> will be invoked to perform argument validation.</para>
  /// <para>May be specified multiple times, with each validator being invoked in an unspecified order.</para>
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> WithValidator<T>(this Option<T> self, Action<Option<T>, OptionResult> validator) {
    self.Validators.Add(result => validator(self, result));
    return self;
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Option{T}"/> is a valid path to a file
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<string> ValidFilePath(this Option<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!File.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Option{T}"/> is a valid path to a directory
  /// </summary>
  /// <returns>This <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<string> ValidDirectoryPath(this Option<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);

      if (!Directory.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }

  /// <summary>
  /// Verifies that the <see langword="string"/> value provided to this <see cref="Option{E}"/> is a valid string representation of the enumeration variant of <typeparamref name="E"/>
  /// </summary>
  /// <returns>This <see cref="Option{E}"/> instance for additional call chaining</returns>
  public static Option<string> ValidEnumVariant<E>(this Option<string> self, bool ignoreCase, bool showVariantsOnError)
    where E : struct, Enum
  {
    return self.WithValidator((self, result) => {
      var value = result.GetValue(self);

      if (Enum.TryParse<E>(value, ignoreCase, out var _)) {
        return;
      }

      var sb = new StringBuilder()
        .AppendLine($"'{value}' is not a valid value for option '{self.Name}'");

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

