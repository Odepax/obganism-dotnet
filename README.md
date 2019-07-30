Obganism .NET
====

![Unreleased](https://img.shields.io/badge/Status-Unreleased-red.svg?style=flat-square)
![Unstable](https://img.shields.io/badge/Status-Unstable-red.svg?style=flat-square)

1. [Installation](#installation)
2. [Usage](#usage)
3. [Roadmap](#roadmap)

Installation
----

WIP here. Will be available via Nuget.

Usage
----

Option A: Procedural style:

```cs
using System.Collections.Generic;
using Obganism.Definitions;
using static Obganism.Parsing.ObganismParsing;

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

```cs
using System.Collections.Generic;
using Obganism.Definitions;
using Obganism.Extensions.Parsing;

namespace Obganism.Example
{
   public static class ExampleProgram
   {
      public static void Main()
      {
         IReadOnlyList<Obgan> output = "cat { name : string }".ConvertFromObganism();
      }
   }
}
```

Option C: DI-ready:

WIP. Probably via another nuget.

Roadmap
----

- [ ] CI tests
- [ ] CD nugets
- [ ] Badges
- [ ] Parsing
