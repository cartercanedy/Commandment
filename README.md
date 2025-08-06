<div align="center">
  <img width="384" src="./res/commandment.png" />

  <br>

  # Commandment
  ### *Let there be a good command-line interface*

</div>

<br>

## `Commandment` provides fluent builder extension methods for the new `System.CommandLine` API

## Add to your project
```powershell
$> dotnet add package Commandment
```

## Build a fluent and composable CLI
```csharp
using System.CommandLine;
using Commandment;

// Create build a typed argument ...
Option<uint> fooOpt = new("--foo", "-f")
  .WithDescription("Foo")
  .NonZero() // ... with convenient numeric validation methods
  .Required();

enum MyEnum {
  Foo,
  Bar,
  Baz
}

// Create an option that takes a variant of `MyEnum` ...
Option<MyEnum> barOpt = new("--bar", "-b")
  .WithDescription("Bar")
  .ValidEnumVariant(ignoreCase: true, showVariantsOnError: true) // ... with configurable parsing and validation
  .Optional()
  .ZeroOrOneArg()
  .WithDefaultValue(MyEnum.Baz);

// Create an option that takes a string ...
Option<string> bazOpt = new("--baz", "-B")
  .WithDescription("Baz")
  .ValidFilePath() // ... with file/directory path validation
  .Required(true);

// Put it all together
RootCommand cli = new RootCommand("MyCLI")
  .AddOpt(fooOpt)
  .AddOpt(barOpt)
  .WithAsyncAction(async (result, cancelToken) => {
    await Task.CompletedTask;
    return 0;
  })
  .Parse()
  .Invoke();
```

## Future plans:
- [x] Implement fluent extensions
- [ ] Create tests
- [ ] Create a reflection-based declarative builder API
  - [ ] Define attribute API for `Command`, `Subcommand`, and `Option`
  - [ ] Implement reflection-based builder
