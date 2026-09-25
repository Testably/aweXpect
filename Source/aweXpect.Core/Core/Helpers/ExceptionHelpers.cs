using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Core.Helpers;

internal static class ExceptionHelpers
{
	public static void ThrowIfNull(this object? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The {Describe(paramName)} cannot be null."));
		}
	}

	/// <summary>
	///     Quotes the <paramref name="paramName" /> and follows the polarity names, which are adjectives, with a noun.
	/// </summary>
	private static string Describe(string? paramName)
		=> paramName is "expected" or "unexpected" ? $"'{paramName}' value" : $"'{paramName}'";

	public static bool IsDefault<T>(this T value) where T : struct
	{
		bool isDefault = value.Equals(default(T));

		return isDefault;
	}
}
