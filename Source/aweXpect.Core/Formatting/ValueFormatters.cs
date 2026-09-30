using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using aweXpect.Core;
#endif
using System.Net;
using System.Reflection;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Formatting;

/// <summary>
///     Extension formatting options.
/// </summary>
public static partial class ValueFormatters
{
	/// <summary>
	///     Fallback for formatting arbitrary objects.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		object? value,
		FormattingOptions? options = null,
		FormattingContext? context = null)
	{
		StringBuilder stringBuilder = new();
		Format(formatter, stringBuilder, value, options, context);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Fallback for formatting arbitrary objects.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		object? value,
		FormattingOptions? options = null,
		FormattingContext? context = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		// Each typed overload consults the registered formatters itself.
		switch (value)
		{
			case bool boolValue:
				Format(formatter, stringBuilder, boolValue, options);
				return;
			case string stringValue:
				Format(formatter, stringBuilder, stringValue, options);
				return;
			case char charValue:
				Format(formatter, stringBuilder, charValue, options);
				return;
			case Type typeValue:
				Format(formatter, stringBuilder, typeValue, options);
				return;
			case Exception exceptionValue:
				Format(formatter, stringBuilder, exceptionValue, options);
				return;
			case IEnumerable enumerableValue:
				FormatEnumerable(formatter, stringBuilder, enumerableValue, options, context);
				return;
			case HttpStatusCode httpStatusCodeValue:
				Format(formatter, stringBuilder, httpStatusCodeValue, options);
				return;
			case DateTime dateTimeValue:
				Format(formatter, stringBuilder, dateTimeValue, options);
				return;
			case DateTimeOffset dateTimeOffsetValue:
				Format(formatter, stringBuilder, dateTimeOffsetValue, options);
				return;
			case TimeSpan timeSpanValue:
				Format(formatter, stringBuilder, timeSpanValue, options);
				return;
#if NET8_0_OR_GREATER
			case DateOnly dateOnlyValue:
				Format(formatter, stringBuilder, dateOnlyValue, options);
				return;
			case TimeOnly timeOnlyValue:
				Format(formatter, stringBuilder, timeOnlyValue, options);
				return;
#endif
			case Guid guidValue:
				Format(formatter, stringBuilder, guidValue, options);
				return;
			case Enum enumValue:
				Format(formatter, stringBuilder, enumValue, options);
				return;
			case double doubleValue:
				Format(formatter, stringBuilder, doubleValue, options);
				return;
			case float floatValue:
				Format(formatter, stringBuilder, floatValue, options);
				return;
#if NET8_0_OR_GREATER
			case Half halfValue:
				Format(formatter, stringBuilder, halfValue, options);
				return;
			case NFloat nFloatValue:
				Format(formatter, stringBuilder, nFloatValue, options);
				return;
#endif
			case decimal decimalValue:
				Format(formatter, stringBuilder, decimalValue, options);
				return;
			case int intValue:
				Format(formatter, stringBuilder, intValue, options);
				return;
			case uint uintValue:
				Format(formatter, stringBuilder, uintValue, options);
				return;
			case long longValue:
				Format(formatter, stringBuilder, longValue, options);
				return;
			case ulong ulongValue:
				Format(formatter, stringBuilder, ulongValue, options);
				return;
			case byte byteValue:
				Format(formatter, stringBuilder, byteValue, options);
				return;
			case sbyte sbyteValue:
				Format(formatter, stringBuilder, sbyteValue, options);
				return;
			case short shortValue:
				Format(formatter, stringBuilder, shortValue, options);
				return;
			case ushort ushortValue:
				Format(formatter, stringBuilder, ushortValue, options);
				return;
			case nint nintValue:
				Format(formatter, stringBuilder, nintValue, options);
				return;
			case nuint nuintValue:
				Format(formatter, stringBuilder, nuintValue, options);
				return;
		}

		if (TryFormatWithRegistrations(stringBuilder, value, options))
		{
			return;
		}

#if NET8_0_OR_GREATER
		if (TryGetAsyncEnumerableType(value.GetType(), out Type? asyncEnumerableType))
		{
			Format(formatter, stringBuilder, asyncEnumerableType, options);
			return;
		}
#endif

		FormatObject(stringBuilder, value,
			options ?? FormattingOptions.MultipleLines, context);
	}

	/// <summary>
	///     The placeholder for a value that could not be formatted, because code of the caller threw the
	///     <paramref name="exception" />, so that building a failure message does not throw and hide the failure.
	/// </summary>
	private static string FormatThrownException(string thrower, Exception exception)
	{
		exception = (exception as UserCodeException)?.Exception ?? exception;
		exception = (exception as TargetInvocationException)?.InnerException ?? exception;
		// The registered formatters are bypassed, as one of them might be the thrower.
		StringBuilder exceptionType = new();
		FormatType(exception.GetType(), exceptionType);
		return $"[{thrower} did throw {exceptionType.ToString().PrependAOrAn()}: {exception.Message}]";
	}

	/// <summary>
	///     Appends the <paramref name="value" /> as formatted by the most recently registered formatter that accepts it.
	/// </summary>
	private static bool TryFormatWithRegistrations<T>(
		StringBuilder stringBuilder,
		T value,
		FormattingOptions? options)
		where T : notnull
	{
		ValueFormatter.Registration[] registrations = ValueFormatter.Registrations;
		return registrations.Length > 0 && TryFormatWithRegistrations(stringBuilder, value, options, registrations);
	}

	/// <summary>
	///     Returns the <paramref name="value" /> as formatted by the most recently registered formatter that accepts it.
	/// </summary>
	private static bool TryFormatWithRegistrations<T>(
		T value,
		FormattingOptions? options,
		[NotNullWhen(true)] out string? formattedValue)
		where T : notnull
	{
		formattedValue = null;
		ValueFormatter.Registration[] registrations = ValueFormatter.Registrations;
		if (registrations.Length == 0)
		{
			return false;
		}

		StringBuilder stringBuilder = new();
		if (!TryFormatWithRegistrations(stringBuilder, value, options, registrations))
		{
			return false;
		}

		formattedValue = stringBuilder.ToString();
		return true;
	}

	private static bool TryFormatWithRegistrations(
		StringBuilder stringBuilder,
		object value,
		FormattingOptions? options,
		ValueFormatter.Registration[] registrations)
	{
		int length = stringBuilder.Length;
		try
		{
			for (int i = registrations.Length - 1; i >= 0; i--)
			{
				if (registrations[i].Formatter.TryFormat(stringBuilder, value, options))
				{
					return true;
				}
			}
		}
		catch (Exception exception)
		{
			stringBuilder.Length = length;
			stringBuilder.Append(FormatThrownException("the formatter", exception));
			return true;
		}

		return false;
	}

#if NET8_0_OR_GREATER
	/// <remarks>
	///     The items of an <see cref="IAsyncEnumerable{T}" /> cannot be listed synchronously, and its members only show
	///     the state of the enumeration, like the fields of a compiler-generated async iterator, so it is named by its
	///     type instead.
	/// </remarks>
	private static bool TryGetAsyncEnumerableType(Type type, [NotNullWhen(true)] out Type? asyncEnumerableType)
	{
		asyncEnumerableType = ReflectionFallback.IsSupported
			? type.GetInterfaces().FirstOrDefault(@interface => @interface.IsGenericType &&
			                                                    @interface.GetGenericTypeDefinition() ==
			                                                    typeof(IAsyncEnumerable<>))
			: null;
		return asyncEnumerableType is not null;
	}
#endif
}
