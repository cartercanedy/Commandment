<div align="center">
  <img width="384" src="./res/commandment.png" />

  <br>

  # Commandment
  ### _Let there be a good command-line interface_

</div>

<br>

## `Commandment` provides fluent builder extension methods for the new `System.CommandLine` API

## Add to your project
```powershell
$> dotnet add package Commandment --prerelease
```

## Build a fluent and composable CLI
### _Program.cs_
```csharp
using System.CommandLine;
using Commandment;

// Create build a typed argument ...
var fooOpt = new Option<uint>("--foo", "-f")
  .WithDescription("Foo")
  .NonZero() // ... with convenient numeric validation methods
  .Required();

// Create an option that takes a variant of `MyEnum` ...
var barOpt = new Option<string>("--bar", "-b")
  .WithDescription("Bar")
  .ValidEnumVariant<MyEnum>(ignoreCase: true, showVariantsOnError: true) // ... with configurable parsing and validation
  .Optional()
  .ZeroOrOneArg()
  .WithDefaultValue(MyEnum.Baz.ToString());

// Create an option that takes a string ...
var bazOpt = new Option<string>("--baz", "-B")
  .WithDescription("Baz")
  .ValidFilePath() // ... with file/directory path validation
  .Required(true);

// Put it all together
new RootCommand("My first Commandment-based CLI")
  .AddOpt(fooOpt)
  .AddOpt(barOpt)
  .AddOpt(bazOpt)
  .WithAsyncAction(async (result, cancelToken) => {
    Console.WriteLine($"--foo='{result.GetValue(fooOpt)}'");
    Console.WriteLine($"--bar='{result.GetValue(barOpt)}'");
    Console.WriteLine($"--baz='{result.GetValue(bazOpt)}'");

    await Task.CompletedTask;

    return 0;
  })
  .Parse(["--foo=42", "--bar=Foo", "-BProgram.cs"])
  .Invoke();

enum MyEnum {
  Foo,
  Bar,
  Baz
}
```

### Output:
```
--foo='42'
--bar='Foo'
--baz='Program.cs'
```

<br>

---

## Future plans:
- [x] Implement fluent extensions
- [ ] Create tests
- [ ] Create a reflection-based declarative builder API
  - [ ] Define attribute API for `Command`, `Subcommand`, and `Option`
  - [ ] Implement reflection-based builder
