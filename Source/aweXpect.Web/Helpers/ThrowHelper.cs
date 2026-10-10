using System;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThrowHelper
{
	public static void ThrowIfNull(object? value, string paramName)
	{
		if (value is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The '{paramName}' cannot be null."));
		}
	}

	/// <summary>
	///     Rejects the <paramref name="option" /> when it <paramref name="isAlreadySpecified" />, because the later
	///     value would silently replace the earlier one.
	/// </summary>
	public static void ThrowIfOptionIsAlreadySpecified(bool isAlreadySpecified, string option)
	{
		if (isAlreadySpecified)
		{
			throw Tracing.WriteException(new InvalidOperationException(
				$"{option} cannot be specified more than once."));
		}
	}
}
