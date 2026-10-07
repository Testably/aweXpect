using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Core.Helpers;

internal static class ExceptionHelpers
{
	public static void ThrowIfNull(this object? parameter,
		[CallerArgumentExpression(nameof(parameter))]
		string? paramName = null)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The {Describe(paramName)} cannot be null."));
		}
	}

	/// <summary>
	///     Throws when the <paramref name="type" /> is null or not an exception type, as no exception could ever match it.
	/// </summary>
	public static void ThrowIfNotAnExceptionType(this Type? type,
		[CallerArgumentExpression(nameof(type))]
		string? paramName = null)
	{
		type.ThrowIfNull(paramName);
		if (!typeof(Exception).IsAssignableFrom(type))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be an exception type, but {Formatter.Format(type)} is not.", paramName));
		}
	}

	/// <summary>
	///     Checks if the <paramref name="exception" /> is of the <paramref name="type" /> or a derived type, where an open
	///     generic type matches every exception constructed from it, like <c>ThatObject.Is(Type)</c>.
	/// </summary>
	public static bool IsOfType(this Exception? exception, Type type)
	{
		if (type.IsInstanceOfType(exception))
		{
			return true;
		}

		if (!type.IsGenericTypeDefinition)
		{
			return false;
		}

		for (Type? baseType = exception?.GetType(); baseType is not null; baseType = baseType.BaseType)
		{
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == type)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="exception" /> is exactly of the <paramref name="type" />, where an open generic
	///     type matches every exception constructed directly from it, like <c>ThatObject.IsExactly(Type)</c>.
	/// </summary>
	public static bool IsExactlyOfType(this Exception? exception, Type type)
	{
		Type? actualType = exception?.GetType();
		return actualType == type ||
		       (type.IsGenericTypeDefinition && actualType?.IsGenericType == true &&
		        actualType.GetGenericTypeDefinition() == type);
	}

	/// <summary>
	///     Quotes the <paramref name="paramName" /> and follows the polarity names, which are adjectives, with a noun.
	/// </summary>
	private static string Describe(string? paramName)
		=> paramName is "expected" or "unexpected" ? $"'{paramName}' value" : $"'{paramName}'";
}
