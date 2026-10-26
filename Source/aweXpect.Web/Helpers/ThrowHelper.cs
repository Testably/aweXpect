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

	public static void ThrowIfNullOrWhiteSpace(string? value, string paramName)
	{
		ThrowIfNull(value, paramName);
		if (string.IsNullOrWhiteSpace(value))
		{
			throw Tracing.WriteException(new ArgumentException($"The '{paramName}' cannot be empty or whitespace.",
				paramName));
		}
	}

}
