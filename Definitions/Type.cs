using System;
using System.Collections.Generic;
using System.Linq;

namespace Obganism.Definitions
{
	public sealed class Type : IEquatable<Type>
	{
		public string Name { get; }
		public IReadOnlyList<Type> Generics { get; }

		public Type(string name, IEnumerable<Type> generics)
		{
			Name = name ?? string.Empty;
			Generics = new List<Type>(generics ?? new Type[0]);
		}

		public Type(string name, params Type[] generics) : this(name, generics as IEnumerable<Type>)
		{
		}

		public override bool Equals(object other) =>
			!(other is null) && (
				ReferenceEquals(other, this) || (
					   ReferenceEquals(GetType(), other.GetType())
					&& Equals(other as Type)
				)
			);

		public bool Equals(Type other) =>
			ReferenceEquals(this, other) || (
				   !(other is null)
				&& Name.Equals(other.Name, StringComparison.Ordinal)
				&& Generics.SequenceEqual(other.Generics)
			);

		public override int GetHashCode()
		{
			HashCode hashCode = new HashCode();

			hashCode.Add(Name, StringComparer.OrdinalIgnoreCase);

			foreach (Type type in Generics)
			{
				hashCode.Add(type);
			}

			return hashCode.ToHashCode();
		}

		/// <summary>
		/// This method is intended for debug purposes.
		/// Changes will not take part in API versionning.
		/// </summary>
		public override string ToString() =>
			$"{ Name }{ (Generics.Count == 0 ? "" : $"({ string.Join(",", Generics) })") }";
	}
}
