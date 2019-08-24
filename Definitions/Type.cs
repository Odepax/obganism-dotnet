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
			Name = name;
			Generics = new List<Type>(generics);
		}

		public Type(string name, params Type[] generics) : this(name, generics as IEnumerable<Type>)
		{
		}

		public override bool Equals(object? obj) =>
			!(obj is null) && (
				   ReferenceEquals(this, obj)
				|| obj is Type other && Equals(other)
			);

		public bool Equals(Type other) =>
			ReferenceEquals(this, other) || (
				   Name.Equals(other.Name, StringComparison.Ordinal)
				&& Generics.SequenceEqual(other.Generics)
			);

		public override int GetHashCode()
		{
			HashCode hashCode = new HashCode();

			hashCode.Add(Name, StringComparer.OrdinalIgnoreCase);
			hashCode.AddAll(Generics);

			return hashCode.ToHashCode();
		}

		/// <summary>
		///
		/// This method is intended for debug purposes.
		/// It will not take part in API versionning.
		///
		/// </summary>
		public override string ToString() =>
			$"{ Name }{ (Generics.Count == 0 ? "" : $"({ string.Join(",", Generics) })") }";
	}
}
