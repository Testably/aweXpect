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
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The {Describe(paramName)} cannot be null."));
		}
	}

	/// <summary>
	///     Quotes the <paramref name="paramName" /> and follows the polarity names, which are adjectives, with a noun.
	/// </summary>
	/// <remarks>
	///     Mirrors the null message of aweXpect.Core; keep the wording identical.
	/// </remarks>
	private static string Describe(string paramName)
		=> paramName is "expected" or "unexpected" ? $"'{paramName}' value" : $"'{paramName}'";
}
