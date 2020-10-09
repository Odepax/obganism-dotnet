using System;
using System.Collections.Generic;

namespace Obganism
{
	public static class ObganismSerializer
	{
		public static IReadOnlyList<ObganismObject> Deserialize(string source) =>
			new ParsingContext { Source = source }.ReadObjects();
	}

	public class ObganismException : Exception
	{
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
