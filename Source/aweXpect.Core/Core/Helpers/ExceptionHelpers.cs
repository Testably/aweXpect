using System;
using System.Runtime.CompilerServices;
using aweXpect.Customization;

namespace aweXpect.Core.Helpers;

internal static class ExceptionHelpers
{
	public static void ThrowIfNull(this object? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentNullException(paramName, $"The {paramName} cannot be null.");
		}
	}

	public static TException LogTrace<TException>(this TException exception)
		where TException : Exception
	{
		Customize.aweXpect.TraceWriter.Value?.WriteException(exception);
		return exception;
	}

	public static bool IsDefault<T>(this T value) where T : struct
	{
		bool isDefault = value.Equals(default(T));

		return isDefault;
	}
}
