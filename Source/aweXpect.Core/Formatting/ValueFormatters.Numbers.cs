using System.Globalization;
using System.Text;
#if NET8_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		byte value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"byte {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		byte value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("byte ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		byte? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		byte? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		sbyte value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"sbyte {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		sbyte value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("sbyte ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		sbyte? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		sbyte? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		short value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"short {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		short value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("short ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		short? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		short? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		ushort value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"ushort {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		ushort value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("ushort ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		ushort? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		ushort? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		int value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"int {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		int value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("int ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		int? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		int? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		uint value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"uint {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		uint value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("uint ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		uint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		uint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		long value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"long {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		long value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("long ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		long? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		long? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		ulong value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"ulong {value.ToString(CultureInfo.InvariantCulture)}",
			_ => value.ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		ulong value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("ulong ");
		}

		stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		ulong? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		ulong? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		float value,
		FormattingOptions? options = null)
		=> (options?.IncludeType, value) switch
		{
			(true, float.NegativeInfinity) => "float -\u221e",
			(true, float.PositiveInfinity) => "float +\u221e",
			(true, float.MinValue) => "float.MinValue",
			(true, float.MaxValue) => "float.MaxValue",
			(true, _) => $"float {value.ToString("0.0###########################", CultureInfo.InvariantCulture)}",
			(_, float.NegativeInfinity) => "-\u221e",
			(_, float.PositiveInfinity) => "+\u221e",
			(_, float.MinValue) => "float.MinValue",
			(_, float.MaxValue) => "float.MaxValue",
			(_, _) => value.ToString("0.0###########################", CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		float value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(Format(formatter, value, options));

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		float? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		float? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		double value,
		FormattingOptions? options = null)
		=> (options?.IncludeType, value) switch
		{
			(true, double.NegativeInfinity) => "double -\u221e",
			(true, double.PositiveInfinity) => "double +\u221e",
			(true, double.MinValue) => "double.MinValue",
			(true, double.MaxValue) => "double.MaxValue",
			(true, _) => $"double {value.ToString("0.0###########################", CultureInfo.InvariantCulture)}",
			(_, double.NegativeInfinity) => "-\u221e",
			(_, double.PositiveInfinity) => "+\u221e",
			(_, double.MinValue) => "double.MinValue",
			(_, double.MaxValue) => "double.MaxValue",
			(_, _) => value.ToString("0.0###########################", CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		double value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(Format(formatter, value, options));

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		double? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		double? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		Half value,
		FormattingOptions? options = null)
	{
		if (Half.IsNegativeInfinity(value))
		{
			return options?.IncludeType == true ? "Half -\u221e" : "-\u221e";
		}

		if (Half.IsPositiveInfinity(value))
		{
			return options?.IncludeType == true ? "Half +\u221e" : "+\u221e";
		}

		if (value == Half.MinValue)
		{
			return "Half.MinValue";
		}

		if (value == Half.MaxValue)
		{
			return "Half.MaxValue";
		}

		// The custom format "0.0###\u2026" would render digits beyond the precision of a Half (11.3 as "11.297").
		string formattedValue = value.ToString(CultureInfo.InvariantCulture);
		if (Half.IsFinite(value) && formattedValue.IndexOfAny(['.', 'E']) < 0)
		{
			formattedValue += ".0";
		}

		if (options?.IncludeType == true)
		{
			return $"Half {formattedValue}";
		}

		return formattedValue;
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Half value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(Format(formatter, value, options));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		Half? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			return ValueFormatter.NullString;
		}

		return Format(formatter, value.Value, options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Half? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		NFloat value,
		FormattingOptions? options = null)
	{
		if (NFloat.IsNegativeInfinity(value))
		{
			return options?.IncludeType == true ? "NFloat -∞" : "-∞";
		}

		if (NFloat.IsPositiveInfinity(value))
		{
			return options?.IncludeType == true ? "NFloat +∞" : "+∞";
		}

		if (options?.IncludeType == true)
		{
			return $"NFloat {value.ToString(CultureInfo.InvariantCulture)}";
		}

		return value.ToString(CultureInfo.InvariantCulture);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		NFloat value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(Format(formatter, value, options));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		NFloat? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			return ValueFormatter.NullString;
		}

		return Format(formatter, value.Value, options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		NFloat? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}
#endif

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		decimal value,
		FormattingOptions? options = null)
		=> (options?.IncludeType, value) switch
		{
			(true, decimal.MinValue) => "decimal.MinValue",
			(true, decimal.MaxValue) => "decimal.MaxValue",
			(true, _) => $"decimal {value.ToString("0.0###########################", CultureInfo.InvariantCulture)}",
			(_, decimal.MinValue) => "decimal.MinValue",
			(_, decimal.MaxValue) => "decimal.MaxValue",
			(_, _) => value.ToString("0.0###########################", CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		decimal value,
		FormattingOptions? options = null)
		=> stringBuilder.Append(Formatter.Format(value, options));

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		decimal? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		decimal? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		nint value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"nint {((long)value).ToString(CultureInfo.InvariantCulture)}",
			_ => ((long)value).ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		nint value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("nint ");
		}

		stringBuilder.Append(((long)value).ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		nint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		nint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		nuint value,
		FormattingOptions? options = null)
		=> options?.IncludeType switch
		{
			true => $"nuint {((ulong)value).ToString(CultureInfo.InvariantCulture)}",
			_ => ((ulong)value).ToString(CultureInfo.InvariantCulture),
		};

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		nuint value,
		FormattingOptions? options = null)
	{
		if (options?.IncludeType == true)
		{
			stringBuilder.Append("nuint ");
		}

		stringBuilder.Append(((ulong)value).ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		nuint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
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
		nuint? value,
		FormattingOptions? options = null)
	{
		if (value is null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		Format(formatter, stringBuilder, value.Value, options);
	}
}
