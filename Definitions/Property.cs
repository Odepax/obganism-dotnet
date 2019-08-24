using System;

namespace Obganism.Definitions
{
	public sealed class Property : IEquatable<Property>
	{
		public string Name { get; }
		public Type Type { get; }

		public Property(string name, Type type)
		{
			Name = name;
			Type = type;
		}

		public override bool Equals(object? obj) =>
			!(obj is null) && (
				   ReferenceEquals(this, obj)
				|| obj is Property other && Equals(other)
			);

		public bool Equals(Property other) =>
			ReferenceEquals(this, other) || (
				   Name.Equals(other.Name, StringComparison.Ordinal)
				&& Type.Equals(other.Type)
			);

		public override int GetHashCode()
		{
			HashCode hashCode = new HashCode();

			hashCode.Add(Name, StringComparer.OrdinalIgnoreCase);
			hashCode.Add(Type);

			return hashCode.ToHashCode();
		}

		/// <summary>
		///
		/// This method is intended for debug purposes.
		/// It will not take part in API versionning.
		///
		/// </summary>
		public override string ToString() =>
			$"{ Name }:{ Type }";
	}
}
