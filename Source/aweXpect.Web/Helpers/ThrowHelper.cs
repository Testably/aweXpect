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
}
