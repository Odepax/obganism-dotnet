using System;
using System.Collections.Generic;

namespace Obganism
{
	public static class ObganismSerializer
	{
		/// <summary>
		/// Converts Obganism source code into CLR objects.
		/// </summary>
		///
		/// <param name="source">
		/// Some Obganism source code as per
		/// <see href="https://github.com/Odepax/obganism-lang/wiki/Obganism-1.1"/>.
		/// </param>
		///
		/// <exception cref="ObganismParsingException">
		/// Thrown when the <paramref name="source"/> code isn't valid Obganism.
		/// </exception>
		public static IReadOnlyList<ObganismObject> Deserialize(string source) =>
			new ParsingContext { Source = source }.ReadObjects();
	}

	public class ObganismException : Exception
	{
		/// <summary>
		/// The 0-based position in the source code string at which the error was detected.
		/// </summary>
		public int Position;

		internal ObganismException(int position, string message, Exception innerException = default) : base(message, innerException)
		{
			Position = position;
		}
	}

	public class ObganismObject
	{
		public ObganismType Type;
		public IReadOnlyList<ObganismProperty> Properties;
		public IReadOnlyList<ObganismModifier> Modifiers;
	}

	public class ObganismType
	{
		public string Name;
		public IReadOnlyList<ObganismType> Generics;
	}

	public class ObganismProperty
	{
		public string Name;
		public ObganismType Type;
		public IReadOnlyList<ObganismModifier> Modifiers;
	}

	public class ObganismModifier
	{
		public string Name;
		public IReadOnlyList<ObganismModifierParameter> Parameters;
	}

	public abstract class ObganismModifierParameter
	{
		public class Integer : ObganismModifierParameter { public int Value; }
		public class Real : ObganismModifierParameter { public float Value; }
		public class String : ObganismModifierParameter { public string Value; }
		public class Name : ObganismModifierParameter { public string Value; }
		public class Type : ObganismModifierParameter { public ObganismType Value; }
	}
}
