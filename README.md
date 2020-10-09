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
         ReadOnlyList<ObganismObject> output = ObganismSerializer.Deserialize(source);
      }
   }
}
```
