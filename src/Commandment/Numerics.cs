using System.CommandLine;
using System.Numerics;

namespace Commandment;

public static class OptionNumericalExtensions {
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
}
