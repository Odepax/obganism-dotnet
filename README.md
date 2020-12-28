Obganism .NET
====

![NuGet](https://img.shields.io/nuget/v/Obganism?label=NuGet&logo=nuget)
![Tests](https://github.com/Odepax/obganism-dotnet/workflows/tests/badge.svg)

1. [Installation](#installation)
2. [Usage](#usage)

Installation
----

Install [Obganism](https://www.nuget.org/packages/Obganism/) from NuGet.

Usage
----

The only method you have to care about is `Obganism.ObganismDocument.Parse(string)`. It can be seen as the equivalent of `System.Text.Json.JsonDocument.Parse(string)` or `Hjson.HjsonValue.Parse(string)`.

```cs
using System.Collections.Generic;
using Obganism;

namespace Obganism.Example
{
   public static class ExampleProgram
   {
      public static void Main()
      {
         string source = "cat { name : string }";
         ReadOnlyList<ObganismObject> output = ObganismDocument.Parse(source);
      }
   }
}
```

`ObganismDocument.Parse()` returns .NET objects that map the concepts explained in the [language specifications](https://github.com/Odepax/obganism-lang/wiki).

The interface of the library lives in [Lib.cs](./Obganism/Lib.cs).
