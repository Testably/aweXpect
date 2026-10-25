using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Json;

/// <summary>
///     The readers of the typed JSON values.
/// </summary>
/// <remarks>
///     The values are read with the methods of <see cref="JsonElement" /> instead of by reflection, so that they are
///     trimming and AOT friendly.
/// </remarks>
internal static class JsonValueReaders
{
	public static readonly JsonValueReader<string> String = new(JsonValueKind.String, null, "a string",
		static (JsonElement element, out string value) =>
		{
			value = element.GetString()!;
			return true;
		});

	public static readonly JsonValueReader<bool> Boolean = new(JsonValueKind.True, null, "a boolean",
		static (JsonElement element, out bool value) =>
		{
			value = element.GetBoolean();
			return true;
		});

	public static readonly JsonValueReader<double> Number = new(JsonValueKind.Number, null, "a double", TryGetDouble);

	/// <summary>
	///     The reader of numbers as <typeparamref name="TNumber" />.
	/// </summary>
	/// <exception cref="System.NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static JsonValueReader<TNumber> GetNumber<TNumber>()
		=> NumberReader<TNumber>.Instance ?? throw Tracing.WriteException(new System.NotSupportedException(
			$"The number type {Formatter.Format(typeof(TNumber))} is not supported. Use byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal."));

	/// <summary>
	///     Creates the tolerance of a comparison of <typeparamref name="TNumber" /> values.
	/// </summary>
	/// <remarks>
	///     Call it only for a <typeparamref name="TNumber" /> that <see cref="GetNumber{TNumber}" /> supports.
	/// </remarks>
	public static NumberTolerance<TNumber> CreateNumberTolerance<TNumber>()
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> new(NumberDifference<TNumber>.Calculate);

	/// <remarks>
	///     Newer versions of <c>System.Text.Json</c> read a number beyond the range as infinity, which does not fit either.
	/// </remarks>
	private static bool TryGetDouble(JsonElement element, out double value)
		=> element.TryGetDouble(out value) && !double.IsInfinity(value);

	/// <remarks>
	///     The difference is written out per type, as the oldest target frameworks have no generic math. An overflow of
	///     the difference throws, which the <see cref="NumberTolerance{TNumber}" /> treats as not within the tolerance.
	/// </remarks>
	private static class NumberDifference<TNumber>
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		public static readonly Func<TNumber, TNumber, TNumber?> Calculate =
			(Func<TNumber, TNumber, TNumber?>)Create()!;

		private static object? Create()
			=> For<byte>(Difference) ?? For<sbyte>(Difference) ?? For<short>(Difference) ??
			   For<ushort>(Difference) ?? For<int>(Difference) ?? For<uint>(Difference) ??
			   For<long>(Difference) ?? For<ulong>(Difference) ?? For<float>(Difference) ??
			   For<double>(Difference) ?? For<decimal>(Difference);

		private static object? For<T>(Func<T, T, T?> difference) where T : struct
			=> typeof(TNumber) == typeof(T) ? difference : null;

		private static byte? Difference(byte a, byte e) => (byte)(a > e ? a - e : e - a);
		private static sbyte? Difference(sbyte a, sbyte e) => checked((sbyte)(a > e ? a - e : e - a));
		private static short? Difference(short a, short e) => checked((short)(a > e ? a - e : e - a));
		private static ushort? Difference(ushort a, ushort e) => (ushort)(a > e ? a - e : e - a);
		private static int? Difference(int a, int e) => checked(a > e ? a - e : e - a);
		private static uint? Difference(uint a, uint e) => a > e ? a - e : e - a;
		private static long? Difference(long a, long e) => checked(a > e ? a - e : e - a);
		private static ulong? Difference(ulong a, ulong e) => a > e ? a - e : e - a;
		private static float? Difference(float a, float e) => Math.Abs(a - e);
		private static double? Difference(double a, double e) => Math.Abs(a - e);
		private static decimal? Difference(decimal a, decimal e) => a > e ? a - e : e - a;
	}

	/// <remarks>
	///     The comparisons with <see langword="typeof" /> are resolved when the type is instantiated, so that no reader is
	///     created by reflection.
	/// </remarks>
	private static class NumberReader<TNumber>
	{
		public static readonly JsonValueReader<TNumber>? Instance = (JsonValueReader<TNumber>?)Create();

		private static object? Create()
		{
			if (typeof(TNumber) == typeof(byte))
			{
				return CreateNumber("a byte", static (JsonElement e, out byte v) => e.TryGetByte(out v));
			}

			if (typeof(TNumber) == typeof(sbyte))
			{
				return CreateNumber("an sbyte", static (JsonElement e, out sbyte v) => e.TryGetSByte(out v));
			}

			if (typeof(TNumber) == typeof(short))
			{
				return CreateNumber("a short", static (JsonElement e, out short v) => e.TryGetInt16(out v));
			}

			if (typeof(TNumber) == typeof(ushort))
			{
				return CreateNumber("a ushort", static (JsonElement e, out ushort v) => e.TryGetUInt16(out v));
			}

			if (typeof(TNumber) == typeof(int))
			{
				return CreateNumber("an int", static (JsonElement e, out int v) => e.TryGetInt32(out v));
			}

			if (typeof(TNumber) == typeof(uint))
			{
				return CreateNumber("a uint", static (JsonElement e, out uint v) => e.TryGetUInt32(out v));
			}

			if (typeof(TNumber) == typeof(long))
			{
				return CreateNumber("a long", static (JsonElement e, out long v) => e.TryGetInt64(out v));
			}

			if (typeof(TNumber) == typeof(ulong))
			{
				return CreateNumber("a ulong", static (JsonElement e, out ulong v) => e.TryGetUInt64(out v));
			}

			if (typeof(TNumber) == typeof(float))
			{
				return CreateNumber<float>("a float", TryGetSingle);
			}

			if (typeof(TNumber) == typeof(double))
			{
				return CreateNumber<double>("a double", TryGetDouble);
			}

			if (typeof(TNumber) == typeof(decimal))
			{
				return CreateNumber("a decimal", static (JsonElement e, out decimal v) => e.TryGetDecimal(out v));
			}

			return null;
		}

		private static JsonValueReader<T> CreateNumber<T>(string typeName, JsonValueRead<T> read)
			=> new(JsonValueKind.Number, typeName, typeName, read);

		/// <inheritdoc cref="TryGetDouble(JsonElement, out double)" />
		private static bool TryGetSingle(JsonElement element, out float value)
			=> element.TryGetSingle(out value) && !float.IsInfinity(value);
	}
}
