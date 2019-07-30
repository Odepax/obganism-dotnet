using System;
using System.Collections.Generic;
using System.Linq;

namespace Obganism.Definitions
{
	public sealed class Obgan : IEquatable<Obgan>
	{
		public Type Type { get; }
		public IReadOnlyList<Property> Properties { get; }

		public Obgan(Type type, IEnumerable<Property> properties)
		{
			Type = type;
			Properties = new List<Property>(properties);
		}

		public Obgan(Type type, params Property[] properties) : this(type, properties as IEnumerable<Property>)
		{
		}

		public override bool Equals(object? obj) =>
			!(obj is null) && (
				   ReferenceEquals(this, obj)
				|| obj is Obgan other && Equals(other)
			);

		public bool Equals(Obgan other) =>
			ReferenceEquals(this, other) || (
				   Type.Equals(other.Type)
				&& Properties.SequenceEqual(other.Properties)
			);

		public override int GetHashCode()
		{
			HashCode hashCode = new HashCode();

			hashCode.Add(Type);

			foreach (Property property in Properties)
			{
				hashCode.Add(property);
			}

			return hashCode.ToHashCode();
		}

		/// <summary>
		/// This method is intended for debug purposes.
		/// It will not take part in API versionning.
		/// </summary>
		public override string ToString() =>
			$"{ Type }{ (Properties.Count == 0 ? "" : $" {{ { string.Join(", ", Properties) } }}") }";
	}
}
