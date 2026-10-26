using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Helpers;

/// <summary>
///     The match types that an expectation with a tolerance on its items starts with, one shared value per item type.
/// </summary>
/// <remarks>
///     The values never change and read a customizable default tolerance only when they compare, so every expectation
///     can start with the same value.
/// </remarks>
internal static class ObjectToleranceMatchTypeFactory
{
	private static readonly ObjectToleranceMatchType<double, double> Double =
		ObjectMatchType<double>.WithTolerance<double>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<double?, double> NullableDouble =
		ObjectMatchType<double?>.WithTolerance<double>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<float, float> Float =
		ObjectMatchType<float>.WithTolerance<float>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<float?, float> NullableFloat =
		ObjectMatchType<float?>.WithTolerance<float>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<decimal, decimal> Decimal =
		ObjectMatchType<decimal>.WithTolerance<decimal>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<decimal?, decimal> NullableDecimal =
		ObjectMatchType<decimal?>.WithTolerance<decimal>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<byte, byte> Byte =
		ObjectMatchType<byte>.WithTolerance<byte>((a, e, t) => ((ulong)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<byte?, byte> NullableByte =
		ObjectMatchType<byte?>.WithTolerance<byte>((a, e, t) => ((ulong?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<sbyte, sbyte> SByte =
		ObjectMatchType<sbyte>.WithTolerance<sbyte>((a, e, t) => ((long)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<sbyte?, sbyte> NullableSByte =
		ObjectMatchType<sbyte?>.WithTolerance<sbyte>((a, e, t) => ((long?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<short, short> Short =
		ObjectMatchType<short>.WithTolerance<short>((a, e, t) => ((long)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<short?, short> NullableShort =
		ObjectMatchType<short?>.WithTolerance<short>((a, e, t) => ((long?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<ushort, ushort> UShort =
		ObjectMatchType<ushort>.WithTolerance<ushort>((a, e, t) => ((ulong)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<ushort?, ushort> NullableUShort =
		ObjectMatchType<ushort?>.WithTolerance<ushort>((a, e, t) => ((ulong?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<int, int> Int =
		ObjectMatchType<int>.WithTolerance<int>((a, e, t) => ((long)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<int?, int> NullableInt =
		ObjectMatchType<int?>.WithTolerance<int>((a, e, t) => ((long?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<uint, uint> UInt =
		ObjectMatchType<uint>.WithTolerance<uint>((a, e, t) => ((ulong)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<uint?, uint> NullableUInt =
		ObjectMatchType<uint?>.WithTolerance<uint>((a, e, t) => ((ulong?)a).IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<long, long> Long =
		ObjectMatchType<long>.WithTolerance<long>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<long?, long> NullableLong =
		ObjectMatchType<long?>.WithTolerance<long>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<ulong, ulong> ULong =
		ObjectMatchType<ulong>.WithTolerance<ulong>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<ulong?, ulong> NullableULong =
		ObjectMatchType<ulong?>.WithTolerance<ulong>((a, e, t) => a.IsConsideredEqualTo(e, t));

	private static readonly ObjectToleranceMatchType<DateTime, TimeSpan> DateTimeValue =
		ObjectMatchType<DateTime>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<DateTime?, TimeSpan> NullableDateTime =
		ObjectMatchType<DateTime?>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<DateTimeOffset, TimeSpan> DateTimeOffsetValue =
		ObjectMatchType<DateTimeOffset>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<DateTimeOffset?, TimeSpan> NullableDateTimeOffset =
		ObjectMatchType<DateTimeOffset?>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<TimeSpan, TimeSpan> TimeSpanValue =
		ObjectMatchType<TimeSpan>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<TimeSpan?, TimeSpan> NullableTimeSpan =
		ObjectMatchType<TimeSpan?>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

#if NET8_0_OR_GREATER
	private static readonly ObjectToleranceMatchType<DateOnly, TimeSpan> DateOnlyValue =
		ObjectMatchType<DateOnly>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			ToDayString, ThrowHelper.ThrowIfToleranceIsNotWholeDays, DefaultDayTolerance);

	private static readonly ObjectToleranceMatchType<DateOnly?, TimeSpan> NullableDateOnly =
		ObjectMatchType<DateOnly?>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			ToDayString, ThrowHelper.ThrowIfToleranceIsNotWholeDays, DefaultDayTolerance);

	private static readonly ObjectToleranceMatchType<TimeOnly, TimeSpan> TimeOnlyValue =
		ObjectMatchType<TimeOnly>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);

	private static readonly ObjectToleranceMatchType<TimeOnly?, TimeSpan> NullableTimeOnly =
		ObjectMatchType<TimeOnly?>.WithTolerance<TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			defaultTolerance: DefaultTimeTolerance);
#endif

	private static readonly Dictionary<Type, object> TimeMatchTypes = new()
	{
		{
			typeof(DateTime), DateTimeValue
		},
		{
			typeof(DateTime?), NullableDateTime
		},
		{
			typeof(DateTimeOffset), DateTimeOffsetValue
		},
		{
			typeof(DateTimeOffset?), NullableDateTimeOffset
		},
		{
			typeof(TimeSpan), TimeSpanValue
		},
		{
			typeof(TimeSpan?), NullableTimeSpan
		},
#if NET8_0_OR_GREATER
		{
			typeof(DateOnly), DateOnlyValue
		},
		{
			typeof(DateOnly?), NullableDateOnly
		},
		{
			typeof(TimeOnly), TimeOnlyValue
		},
		{
			typeof(TimeOnly?), NullableTimeOnly
		},
#endif
	};

	public static ObjectToleranceMatchType<double, double> CreateDouble() => Double;

	public static ObjectToleranceMatchType<double?, double> CreateNullableDouble() => NullableDouble;

	public static ObjectToleranceMatchType<float, float> CreateFloat() => Float;

	public static ObjectToleranceMatchType<float?, float> CreateNullableFloat() => NullableFloat;

	public static ObjectToleranceMatchType<decimal, decimal> CreateDecimal() => Decimal;

	public static ObjectToleranceMatchType<decimal?, decimal> CreateNullableDecimal() => NullableDecimal;

	public static ObjectToleranceMatchType<byte, byte> CreateByte() => Byte;

	public static ObjectToleranceMatchType<byte?, byte> CreateNullableByte() => NullableByte;

	public static ObjectToleranceMatchType<sbyte, sbyte> CreateSByte() => SByte;

	public static ObjectToleranceMatchType<sbyte?, sbyte> CreateNullableSByte() => NullableSByte;

	public static ObjectToleranceMatchType<short, short> CreateShort() => Short;

	public static ObjectToleranceMatchType<short?, short> CreateNullableShort() => NullableShort;

	public static ObjectToleranceMatchType<ushort, ushort> CreateUShort() => UShort;

	public static ObjectToleranceMatchType<ushort?, ushort> CreateNullableUShort() => NullableUShort;

	public static ObjectToleranceMatchType<int, int> CreateInt() => Int;

	public static ObjectToleranceMatchType<int?, int> CreateNullableInt() => NullableInt;

	public static ObjectToleranceMatchType<uint, uint> CreateUInt() => UInt;

	public static ObjectToleranceMatchType<uint?, uint> CreateNullableUInt() => NullableUInt;

	public static ObjectToleranceMatchType<long, long> CreateLong() => Long;

	public static ObjectToleranceMatchType<long?, long> CreateNullableLong() => NullableLong;

	public static ObjectToleranceMatchType<ulong, ulong> CreateULong() => ULong;

	public static ObjectToleranceMatchType<ulong?, ulong> CreateNullableULong() => NullableULong;

	public static ObjectToleranceMatchType<DateTime, TimeSpan> CreateDateTime() => DateTimeValue;

	public static ObjectToleranceMatchType<DateTime?, TimeSpan> CreateNullableDateTime() => NullableDateTime;

	public static ObjectToleranceMatchType<DateTimeOffset, TimeSpan> CreateDateTimeOffset() => DateTimeOffsetValue;

	public static ObjectToleranceMatchType<DateTimeOffset?, TimeSpan> CreateNullableDateTimeOffset()
		=> NullableDateTimeOffset;

	public static ObjectToleranceMatchType<TimeSpan, TimeSpan> CreateTimeSpan() => TimeSpanValue;

	public static ObjectToleranceMatchType<TimeSpan?, TimeSpan> CreateNullableTimeSpan() => NullableTimeSpan;

	/// <summary>
	///     Returns the match type for values of type <typeparamref name="TItem" />, which applies the default time
	///     tolerance when <typeparamref name="TItem" /> is one of the time types.
	/// </summary>
	/// <remarks>
	///     Serves the expectations whose value type is only a type parameter, so that none of the <c>Create…</c> methods
	///     can be chosen at compile time. Its name must not start with <c>Create</c>, as the generator would pick it up.
	/// </remarks>
	public static ObjectMatchType<TItem> ForValuesOf<TItem>()
		=> TimeMatchTypes.TryGetValue(typeof(TItem), out object? matchType)
			? (ObjectMatchType<TItem>)matchType
			: ObjectMatchType<TItem>.Default;

	private static TimeSpan DefaultTimeTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();

#if NET8_0_OR_GREATER
	public static ObjectToleranceMatchType<DateOnly, TimeSpan> CreateDateOnly() => DateOnlyValue;

	public static ObjectToleranceMatchType<DateOnly?, TimeSpan> CreateNullableDateOnly() => NullableDateOnly;

	public static ObjectToleranceMatchType<TimeOnly, TimeSpan> CreateTimeOnly() => TimeOnlyValue;

	public static ObjectToleranceMatchType<TimeOnly?, TimeSpan> CreateNullableTimeOnly() => NullableTimeOnly;

	/// <remarks>
	///     A date only honours the whole days of the default tolerance, so a shorter one is not named in the text.
	/// </remarks>
	private static TimeSpan DefaultDayTolerance()
		=> TimeSpan.FromDays((int)DefaultTimeTolerance().TotalDays);

	private static string ToDayString(TimeSpan tolerance)
	{
		int days = (int)tolerance.TotalDays;
		return days == 1 ? " ± 1 day" : $" ± {days} days";
	}
#endif
}
