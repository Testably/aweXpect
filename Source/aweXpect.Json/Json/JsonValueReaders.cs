using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Json;

/// <summary>
///     The readers of the typed JSON values.
/// </summary>
/// <remarks>
///     The values are read with the methods of <see cref="JsonElement" /> or parsed from the JSON text instead of by
///     reflection, so that they are trimming and AOT friendly.
/// </remarks>
internal static class JsonValueReaders
{
	/// <summary>
	///     The number types that <see cref="GetNumber{TNumber}" /> supports, for the message of an unsupported one.
	/// </summary>
	private const string SupportedNumberTypes =
#if NET8_0_OR_GREATER
		"byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint, Int128, UInt128, BigInteger, Half, float, double or decimal";
#else
		"byte, sbyte, short, ushort, int, uint, long, ulong, BigInteger, float, double or decimal";
#endif

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
			$"The number type {Formatter.Format(typeof(TNumber))} is not supported. Use {SupportedNumberTypes}."));

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
			   For<long>(Difference) ?? For<ulong>(Difference) ?? For<BigInteger>(Difference) ??
			   For<float>(Difference) ?? For<double>(Difference) ?? For<decimal>(Difference)
#if NET8_0_OR_GREATER
			   ?? For<nint>(Difference) ?? For<nuint>(Difference) ?? For<Int128>(Difference) ??
			   For<UInt128>(Difference) ?? For<Half>(Difference)
#endif
			;

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
		private static BigInteger? Difference(BigInteger a, BigInteger e) => BigInteger.Abs(a - e);
		private static float? Difference(float a, float e) => Math.Abs(a - e);
		private static double? Difference(double a, double e) => Math.Abs(a - e);
		private static decimal? Difference(decimal a, decimal e) => a > e ? a - e : e - a;
#if NET8_0_OR_GREATER
		private static nint? Difference(nint a, nint e) => checked(a > e ? a - e : e - a);
		private static nuint? Difference(nuint a, nuint e) => a > e ? a - e : e - a;
		private static Int128? Difference(Int128 a, Int128 e) => checked(a > e ? a - e : e - a);
		private static UInt128? Difference(UInt128 a, UInt128 e) => a > e ? a - e : e - a;
		private static Half? Difference(Half a, Half e) => Half.Abs(a - e);
#endif
	}

	/// <remarks>
	///     The comparisons with <see langword="typeof" /> are resolved when the type is instantiated, so that no reader is
	///     created by reflection.
	/// </remarks>
	private static class NumberReader<TNumber>
	{
		public static readonly JsonValueReader<TNumber>? Instance = Create();

		private static JsonValueReader<TNumber>? Create()
			=> For<byte>("a byte", static (JsonElement e, out byte v) => e.TryGetByte(out v)) ??
			   For<sbyte>("an sbyte", static (JsonElement e, out sbyte v) => e.TryGetSByte(out v)) ??
			   For<short>("a short", static (JsonElement e, out short v) => e.TryGetInt16(out v)) ??
			   For<ushort>("a ushort", static (JsonElement e, out ushort v) => e.TryGetUInt16(out v)) ??
			   For<int>("an int", static (JsonElement e, out int v) => e.TryGetInt32(out v)) ??
			   For<uint>("a uint", static (JsonElement e, out uint v) => e.TryGetUInt32(out v)) ??
			   For<long>("a long", static (JsonElement e, out long v) => e.TryGetInt64(out v)) ??
			   For<ulong>("a ulong", static (JsonElement e, out ulong v) => e.TryGetUInt64(out v)) ??
			   For<BigInteger>("a BigInteger", TryGetBigInteger) ??
			   For<float>("a float", TryGetSingle) ??
			   For<double>("a double", TryGetDouble) ??
			   For<decimal>("a decimal", static (JsonElement e, out decimal v) => e.TryGetDecimal(out v))
#if NET8_0_OR_GREATER
			   ?? For<nint>("an nint", TryParseInteger) ??
			   For<nuint>("an nuint", TryParseInteger) ??
			   For<Int128>("an Int128", TryParseInteger) ??
			   For<UInt128>("a UInt128", TryParseInteger) ??
			   For<Half>("a Half", TryGetHalf)
#endif
			;

		private static JsonValueReader<TNumber>? For<T>(string typeName, JsonValueRead<T> read)
			=> typeof(TNumber) == typeof(T)
				? (JsonValueReader<TNumber>)(object)new JsonValueReader<T>(JsonValueKind.Number, typeName, typeName, read)
				: null;

		/// <inheritdoc cref="TryGetDouble(JsonElement, out double)" />
		private static bool TryGetSingle(JsonElement element, out float value)
			=> element.TryGetSingle(out value) && !float.IsInfinity(value);

		/// <remarks>
		///     Like the methods of <see cref="JsonElement" /> for the integer types, a number with a fraction or an exponent
		///     does not fit.
		/// </remarks>
		private static bool TryGetBigInteger(JsonElement element, out BigInteger value)
			=> BigInteger.TryParse(element.GetRawText(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture,
				out value);

#if NET8_0_OR_GREATER
		/// <inheritdoc cref="TryGetBigInteger(JsonElement, out BigInteger)" />
		private static bool TryParseInteger<T>(JsonElement element, out T value) where T : IBinaryInteger<T>
			=> T.TryParse(element.GetRawText(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value!);

		/// <inheritdoc cref="TryGetDouble(JsonElement, out double)" />
		private static bool TryGetHalf(JsonElement element, out Half value)
			=> Half.TryParse(element.GetRawText(), NumberStyles.Float, CultureInfo.InvariantCulture, out value) &&
			   !Half.IsInfinity(value);
#endif
	}
}
