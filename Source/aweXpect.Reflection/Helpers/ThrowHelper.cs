using System;
using aweXpect.Core;

namespace aweXpect.Reflection.Helpers;

internal static class ThrowHelper
{
	/// <summary>
	///     Throws an <see cref="ArgumentNullException" /> when the <paramref name="value" /> is <see langword="null" />.
	/// </summary>
	public static void ThrowIfNull(object? value, string paramName)
	{
		if (value is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The '{paramName}' cannot be null."));
		}
	}
}
