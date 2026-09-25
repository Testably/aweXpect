using System;
using System.Globalization;
using System.Text;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		TimeSpan value,
		FormattingOptions? options = null)
	{
		StringBuilder stringBuilder = new();
		Format(formatter, stringBuilder, value, options);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		TimeSpan value,
		FormattingOptions? options = null)
		=> Format(formatter, stringBuilder, (TimeSpan?)value, options);

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		TimeSpan? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		return Format(formatter, value.Value, options);
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		TimeSpan? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		if (value == TimeSpan.MaxValue)
		{
			stringBuilder.Append("TimeSpan.MaxValue");
			return;
		}

		if (value == TimeSpan.MinValue)
		{
			stringBuilder.Append("TimeSpan.MinValue");
			return;
		}

		if (options?.IncludeType == true)
		{
			stringBuilder.Append("TimeSpan ");
		}

		if (value.Value < TimeSpan.Zero)
		{
			stringBuilder.Append('-');
		}

		TimeSpan absoluteValue = value.Value.Duration();
		bool hasDays = absoluteValue.Days > 0;
		bool hasHours = absoluteValue.Hours > 0;

		if (hasDays)
		{
			stringBuilder.Append(absoluteValue.Days);
			stringBuilder.Append('.');
		}

		if (hasDays || hasHours)
		{
			if (hasDays && absoluteValue.Hours < 10)
			{
				stringBuilder.Append('0');
			}

			stringBuilder.Append(absoluteValue.Hours);
			stringBuilder.Append(':');
		}

		if ((hasDays || hasHours) && absoluteValue.Minutes < 10)
		{
			stringBuilder.Append('0');
		}

		stringBuilder.Append(absoluteValue.Minutes);
		stringBuilder.Append(':');

		if (absoluteValue.Seconds < 10)
		{
			stringBuilder.Append('0');
		}

		stringBuilder.Append(absoluteValue.Seconds);
		AppendFractionOfSecond(stringBuilder, absoluteValue);
	}

	private static void AppendFractionOfSecond(StringBuilder stringBuilder, TimeSpan absoluteValue)
	{
		long fractionTicks = absoluteValue.Ticks % TimeSpan.TicksPerSecond;
		if (fractionTicks % TimeSpan.TicksPerMillisecond != 0)
		{
			stringBuilder.Append('.');
			stringBuilder.Append(fractionTicks.ToString("0000000", CultureInfo.InvariantCulture).TrimEnd('0'));
		}
		else if (absoluteValue.Milliseconds > 0)
		{
			stringBuilder.Append('.');
			stringBuilder.Append(absoluteValue.Milliseconds.ToString("000"));
		}
	}
}
