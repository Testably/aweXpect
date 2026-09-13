using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Text;

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

		if (!ValueFormatter.RegisteredValueFormatters.IsEmpty &&
		    ValueFormatter.RegisteredValueFormatters.Any(item
			    => item.Value.TryFormat(stringBuilder, value, options)))
		{
			return;
		}

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
			case IEnumerable enumerableValue:
				Format(formatter, stringBuilder, enumerableValue, options);
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

		FormatObject(stringBuilder, value,
			options ?? FormattingOptions.MultipleLines, context);
	}
}
