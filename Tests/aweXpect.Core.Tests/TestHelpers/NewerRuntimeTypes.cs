#if NETFRAMEWORK
using System.Globalization;

namespace System
{
	/// <summary>
	///     Stands in for the type of .NET 5 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct Half(double value) : IFormattable
	{
		private readonly double _value = value;

		public static explicit operator Half(double value) => new(value);

		public override string ToString() => _value.ToString(CultureInfo.CurrentCulture);

		public string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString(format, formatProvider);
	}

	/// <summary>
	///     Stands in for the type of .NET 7 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct Int128(long value)
	{
		private readonly long _value = value;

		public static explicit operator Int128(long value) => new(value);

		public override string ToString() => _value.ToString(CultureInfo.CurrentCulture);
	}

	/// <summary>
	///     Stands in for the type of .NET 7 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct UInt128(ulong value)
	{
		private readonly ulong _value = value;

		public static explicit operator UInt128(ulong value) => new(value);

		public override string ToString() => _value.ToString(CultureInfo.CurrentCulture);
	}

	/// <summary>
	///     Stands in for the type of .NET 6 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct DateOnly(int year, int month, int day) : IFormattable
	{
		private readonly DateTime _value = new(year, month, day);

		public override string ToString() => _value.ToString("d", CultureInfo.CurrentCulture);

		public string ToString(string? format, IFormatProvider? formatProvider)
			=> _value.ToString(format == "o" ? "yyyy-MM-dd" : format, formatProvider);
	}

	/// <summary>
	///     Stands in for the type of .NET 6 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct TimeOnly(int hour, int minute, int second, int millisecond) : IFormattable
	{
		private readonly DateTime _value = new(1, 1, 1, hour, minute, second, millisecond);

		public override string ToString() => _value.ToString("t", CultureInfo.CurrentCulture);

		public string ToString(string? format, IFormatProvider? formatProvider)
			=> _value.ToString(format == "o" ? "HH:mm:ss.fffffff" : format, formatProvider);
	}
}

namespace System.Runtime.InteropServices
{
	/// <summary>
	///     Stands in for the type of .NET 6 and later, which the netstandard2.0 build can only recognize by its name.
	/// </summary>
	internal readonly struct NFloat(double value) : IFormattable
	{
		private readonly double _value = value;

		public static explicit operator NFloat(double value) => new(value);

		public override string ToString() => _value.ToString(CultureInfo.CurrentCulture);

		public string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString(format, formatProvider);
	}
}
#endif
