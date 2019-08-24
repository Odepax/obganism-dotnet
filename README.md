Obganism .NET
====

![Unreleased](https://img.shields.io/badge/Status-Unreleased-red.svg?style=flat-square)
![Unstable](https://img.shields.io/badge/Status-Unstable-red.svg?style=flat-square)

<!-- ![NuGet: Obganism.Parsing](https://img.shields.io/nuget/v/Obganism.Parsing?style=flat-square&label=NuGet&logo=nuget) -->

1. [Installation](#installation)
2. [Usage](#usage)
3. [Roadmap](#roadmap)

Installation
----

WIP here. Will be available via Nuget.

<!-- Install [Obganism.Parsing](https://www.nuget.org/packages/Obganism.Parsing/) from NuGet. -->

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

Roadmap
----

- [x] 0.1.0 &mdash; Support for [Obganism 1.0](https://github.com/Odepax/obganism-lang/wiki/Obganism-1.0).
- [ ] 0.2.0 &mdash; Error messages.
<!-- - [ ] 0.3.0 &mdash; Support for [Obganism 1.1](https://github.com/Odepax/obganism-lang/wiki/Obganism-1.1). -->
