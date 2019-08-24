using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Obganism.Definitions
{
	internal static class AddAllHashCodeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AddAll<T>(this ref HashCode @this, IEnumerable<T> values)
		{
			foreach (T value in values)
			{
				@this.Add(value);
			}
		}
	}
}
