using System.CommandLine;
using System.Numerics;

namespace Commandment;

public static class ArgumentNumericExtensions {
  /// <summary>
  /// This argument must be a non-zero value
  /// (i.e. <c> <paramref name="lhs"/> != 0 </c>)
  /// </summary>
  /// <returns>The same argument instance for additional call chaining</returns>
  public static Argument<T> NonZero<T>(this Argument<T> self) where T : INumber<T> {
    self.Validators.Add(result => {
      if (result.GetValue(self) == T.CreateChecked(0)) {
        result.AddError($"{self.Name} cannot be zero");
      }
    });

    return self;
  }

  /// <summary>
  /// This argument must be greater than <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &gt; <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same argument instance for additional call chaining</returns>
  public static Argument<T> GreaterThan<T>(this Argument<T> lhs, T rhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (result.GetValue(lhs) is T value && value <= rhs) {
        result.AddError($"{lhs.Name} cannot be less than or equal to {rhs}");
      }
    });

    return lhs;
  }

  /// <summary>
  /// This argument must be greater than or equal to <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &gt;= <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same argument instance for additional call chaining</returns>
  public static Argument<T> GreaterThanOrEqualTo<T>(this Argument<T> self, T gteOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      if (result.GetValue(self) is T value && value < gteOperand) {
        result.AddError($"{self.Name} cannot be less than {gteOperand}");
      }
    });

    return self;
  }

  /// <summary>
  /// This argument must be less than <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &lt; <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same argument instance for additional call chaining</returns>
  public static Argument<T> LessThan<T>(this Argument<T> self, T ltOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      if (result.GetValue(self) is T value && value >= ltOperand) {
        result.AddError($"{self.Name} cannot be greater than or equal to {ltOperand}");
      }
    });

    return self;
  }

  /// <summary>
  /// This argument must be less than or equal to <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &lt;= <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same argument instance for additional call chaining</returns>
  public static Argument<T> LessThanOrEqualTo<T>(this Argument<T> self, T lteOperand) where T : INumber<T> {
    self.Validators.Add(result => {
      var raw = result.GetValue(self);
      if (raw is T value && value > lteOperand) {
        result.AddError($"{self.Name} cannot be greater than {lteOperand}");
      }
    });

    return self;
  }
}

public static class OptionNumericExtensions {
  /// <summary>
  /// This option must be a non-zero value
  /// (i.e. <c> <paramref name="lhs"/> != 0 </c>)
  /// </summary>
  /// <returns>The same option instance for additional call chaining</returns>
  public static Option<T> NonZero<T>(this Option<T> lhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (lhs.Required && result.GetValue(lhs) is T value && value == T.CreateChecked(0)) {
        result.AddError($"{lhs.Name} cannot be zero");
      }
    });

    return lhs;
  }

  /// <summary>
  /// This option must be less than <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &lt; <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same option instance for additional call chaining</returns>
  public static Option<T> GreaterThan<T>(this Option<T> lhs, T rhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (lhs.Required && result.GetValue(lhs) is T value && value <= rhs) {
        result.AddError($"{lhs.Name} cannot be less than or equal to {rhs}");
      }
    });

    return lhs;
  }

  /// <summary>
  /// This option must be greater than or equal to <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &gt;= <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same option instance for additional call chaining</returns>
  public static Option<T> GreaterThanOrEqualTo<T>(this Option<T> lhs, T rhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (lhs.Required && result.GetValue(lhs) is T value && value < rhs) {
        result.AddError($"{lhs.Name} cannot be less than {rhs}");
      }
    });

    return lhs;
  }

  /// <summary>
  /// This option must be less than <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &lt; <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same option instance for additional call chaining</returns>
  public static Option<T> LessThan<T>(this Option<T> lhs, T rhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (lhs.Required && result.GetValue(lhs) is T value && value >= rhs) {
        result.AddError($"{lhs.Name} cannot be greater than or equal to {rhs}");
      }
    });

    return lhs;
  }

  /// <summary>
  /// This option must be less than or equal to <paramref name="rhs"/>
  /// (i.e. <c> <paramref name="lhs"/> &lt;= <paramref name="rhs"/> </c>)
  /// </summary>
  /// <returns>The same <see cref="Option{T}"/> instance for additional call chaining</returns>
  public static Option<T> LessThanOrEqualTo<T>(this Option<T> lhs, T rhs) where T : INumber<T> {
    lhs.Validators.Add(result => {
      if (lhs.Required && result.GetValue(lhs) is T value && value > rhs) {
        result.AddError($"{lhs.Name} cannot be greater than {rhs}");
      }
    });

    return lhs;
  }
}
