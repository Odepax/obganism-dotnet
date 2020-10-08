Obganism .NET
====

on:
  push:
    branches: [ "test-gh-actions" ]
    tags: [ "[0-9]+.[0-9]+.[0-9]+" ] # OR
    tags-ignore: [ "tst-*" ]

![NuGet: Obganism.Parsing](https://img.shields.io/nuget/v/Obganism.Parsing?style=flat-square&label=NuGet&logo=nuget)
![Coverage](https://img.shields.io/coveralls/github/Odepax/obganism-dotnet?style=flat-square&label=Coverage&logo=coveralls)
![Tests](https://github.com/Odepax/obganism-dotnet/workflows/Tests/badge.svg)

1. [Installation](#installation)
2. [Usage](#usage)
3. [Development](#development)

Installation
----

Install [Obganism.Parsing](https://www.nuget.org/packages/Obganism.Parsing/) from NuGet.

Usage
----

Option A: Procedural style:

```cs
using System.Collections.Generic; // IReadOnlyList<T>
using Obganism.Definitions; // Obgan

using static Obganism.Parsing.ObganismParsing; // ConvertFromObganism()

namespace Obganism.Example
{
   public static class ExampleProgram
   {
      public static void Main()
      {
         IReadOnlyList<Obgan> output = ConvertFromObganism("cat { name : string }");
      }
   }
}
```

Option B: Fluent style:

WIP.

Option C: DI-ready:

WIP. Probably via another nuget.

Development
----

### Generate Code Coverage Statistics

```
dotnet add "./Parsing.Tests" package "coverlet.collector"
dotnet tool install "dotnet-reportgenerator-globaltool" --tool-path "./bin/Tools"
```

### Generate Code Coverage HTML Report

```
dotnet test --collect "XPlat Code Coverage" --results-directory "./bin/Coverage"
./bin/Tools/reportgenerator.exe -reports:./bin/Coverage/*/* -targetdir:./bin/Coverage/Report -historydir:./bin/Coverage/History
```

### Publish to Nuget

```
dotnet pack --output "./bin/Nuget" --include-symbols --include-source --configuration "Release"
dotnet nuget push "./bin/Nuget/*.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "..."
```
