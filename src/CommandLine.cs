using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Numerics;

namespace Commandment;

public static class CommandExtensions {
  public static Command AddOpt<T>(this Command self, Option<T> opt) {
    self.Add(opt);
    return self;
  }
}

public static class OptionExtensions {
  public static Option<T> NonZero<T>(this Option<T> self) where T : INumber<T> {
    self.Validators.Add(result => {
      var zero = T.CreateChecked(0);
      if (self.Required && result.GetValue(self) is T value && value == zero) {
        result.AddError($"{self.Name} cannot be zero");
      }
    });

    return self;
  }

  public static Option<T> GreaterThan<T>(this Option<T> self, T gtOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      if (self.Required && result.GetValue(self) is T value && value <= gtOperand) {
        result.AddError($"{self.Name} cannot be less than or equal to {gtOperand}");
      }
    });

    return self;
  }

  public static Option<T> GreaterThanOrEqualTo<T>(this Option<T> self, T gteOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      if (self.Required && result.GetValue(self) is T value && value < gteOperand) {
        result.AddError($"{self.Name} cannot be less than {gteOperand}");
      }
    });

    return self;
  }

  public static Option<T> LessThan<T>(this Option<T> self, T ltOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      if (self.Required && result.GetValue(self) is T value && value >= ltOperand) {
        result.AddError($"{self.Name} cannot be greater than or equal to {ltOperand}");
      }
    });

    return self;
  }

  public static Option<T> LessThanOrEqualTo<T>(this Option<T> self, T lteOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      var raw = result.GetValue(self);
      if (self.Required && raw is T value && value < lteOperand) {
        result.AddError($"{self.Name} cannot be greater than {lteOperand}");
      }
    });

    return self;
  }

  public static Option<T> ZeroOrOneArg<T>(this Option<T> self) {
    self.Arity = ArgumentArity.ZeroOrOne;
    return self;
  }

  public static Option<T> OneArg<T>(this Option<T> self) {
    self.Arity = ArgumentArity.ExactlyOne;
    return self;
  }

  public static Option<T> Required<T>(this Option<T> self) {
    self.Required = true;
    return self;
  }

  public static Option<T> Optional<T>(this Option<T> self) {
    self.Required = false;
    return self;
  }

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

  public static Option<string> ValidPath(this Option<string> self) {
    return self.WithValidator((self, result) => {
      var path = result.GetValue(self);
      if (!Directory.Exists(path)) {
        result.AddError($"Path '{path}' doesn't exist");
      }
    });
  }
}

