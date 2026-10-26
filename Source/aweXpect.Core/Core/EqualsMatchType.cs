#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System.Globalization;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The comparison by <see cref="object.Equals(object)" />, which also considers numbers of different types equal
///     when they have the same value.
/// </summary>
internal static class EqualsMatchType
{
	internal static bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		if (actual is null || expected is null)
		{
			return false;
		}

		if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
		{
			return false;
		}

		if (expected is TActual castedExpected &&
		    UserCode.Invoke(static values => EqualityComparer<TActual>.Default.Equals(values.Actual, values.Expected),
			    (Actual: actual, Expected: castedExpected), static values => UserCode.EqualsOf(values.Actual!)))
		{
			return true;
		}

		if (HasTypedEqualsLikeObjectEquals<TActual>() && expected is TActual)
		{
			return false;
		}

		if (typeof(TActual) == typeof(object) &&
		    AreNumericsEqual(actual, expected))
		{
			return true;
		}

		return UserCode.Invoke(static values => Equals(values.Actual, values.Expected),
			(Actual: actual, Expected: expected), static values => UserCode.EqualsOf(values.Actual!));
	}

	/// <summary>
	///     Whether the <see cref="object.Equals(object)" /> of <typeparamref name="T" /> agrees with its typed
	///     <c>Equals</c>, so that comparing the boxed values cannot differ.
	/// </summary>
	private static bool HasTypedEqualsLikeObjectEquals<T>()
		=> typeof(T).IsPrimitive || typeof(T).IsEnum || typeof(T) == typeof(decimal) ||
		   typeof(T) == typeof(Guid) || typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTimeOffset) ||
		   typeof(T) == typeof(TimeSpan);

	private static bool AreNumericsEqual(object actual, object expected)
	{
		Type expectedType = expected.GetType();
		Type actualType = actual.GetType();

		return actualType != expectedType
		       && IsNumericType(actual)
		       && IsNumericType(expected)
		       && IsEqualWhenConverted(actual, expected)
		       && IsEqualWhenConverted(expected, actual);

		bool IsNumericType(object obj)
		{
			return obj is
#if NET8_0_OR_GREATER
				Int128 or
				UInt128 or
				Half or
#endif
				nint or
				nuint or
				int or
				long or
				float or
				double or
				decimal or
				sbyte or
				byte or
				short or
				ushort or
				uint or
				ulong;
		}
	}

	/// <remarks>
	///     Every number is converted with the checked conversion of generic math, which needs no
	///     <see langword="dynamic" /> binder, unavailable when publishing with Native AOT enabled. A value that does
	///     not fit into the target type throws instead of wrapping around, so it counts as not equal, as with
	///     <see cref="Convert.ChangeType(object, Type, IFormatProvider)" /> on the other target frameworks. The caller
	///     converts in both directions, so a value that loses precision in one direction never counts as equal on
	///     its own. <see cref="Convert" /> cannot convert a native integer, so on the other target frameworks it is
	///     converted as a 64-bit integer instead, which holds every native integer.
	/// </remarks>
	private static bool IsEqualWhenConverted(object source, object target)
	{
		try
		{
#if NET8_0_OR_GREATER
			return source switch
			{
				int number => IsEqualWhenConverted(number, target),
				long number => IsEqualWhenConverted(number, target),
				float number => IsEqualWhenConverted(number, target),
				double number => IsEqualWhenConverted(number, target),
				decimal number => IsEqualWhenConverted(number, target),
				sbyte number => IsEqualWhenConverted(number, target),
				byte number => IsEqualWhenConverted(number, target),
				short number => IsEqualWhenConverted(number, target),
				ushort number => IsEqualWhenConverted(number, target),
				uint number => IsEqualWhenConverted(number, target),
				ulong number => IsEqualWhenConverted(number, target),
				Int128 number => IsEqualWhenConverted(number, target),
				UInt128 number => IsEqualWhenConverted(number, target),
				nint number => IsEqualWhenConverted(number, target),
				nuint number => IsEqualWhenConverted(number, target),
				Half number => IsEqualWhenConverted(number, target),
				_ => false,
			};
#else
			object widenedTarget = WidenNativeInteger(target);
			object? convertedNumber = Convert.ChangeType(WidenNativeInteger(source), widenedTarget.GetType(),
				CultureInfo.InvariantCulture);
			return widenedTarget.Equals(convertedNumber);
#endif
		}
		catch
		{
			return false;
		}
	}

#if !NET8_0_OR_GREATER
	private static object WidenNativeInteger(object number)
		=> number switch
		{
			IntPtr value => value.ToInt64(),
			UIntPtr value => value.ToUInt64(),
			_ => number,
		};
#endif

#if NET8_0_OR_GREATER
	private static bool IsEqualWhenConverted<TSource>(TSource source, object target)
		where TSource : INumberBase<TSource>
		=> target switch
		{
			int number => number.Equals(int.CreateChecked(source)),
			long number => number.Equals(long.CreateChecked(source)),
			float number => number.Equals(float.CreateChecked(source)),
			double number => number.Equals(double.CreateChecked(source)),
			decimal number => number.Equals(decimal.CreateChecked(source)),
			sbyte number => number.Equals(sbyte.CreateChecked(source)),
			byte number => number.Equals(byte.CreateChecked(source)),
			short number => number.Equals(short.CreateChecked(source)),
			ushort number => number.Equals(ushort.CreateChecked(source)),
			uint number => number.Equals(uint.CreateChecked(source)),
			ulong number => number.Equals(ulong.CreateChecked(source)),
			Int128 number => number.Equals(Int128.CreateChecked(source)),
			UInt128 number => number.Equals(UInt128.CreateChecked(source)),
			nint number => number.Equals(nint.CreateChecked(source)),
			nuint number => number.Equals(nuint.CreateChecked(source)),
			Half number => number.Equals(Half.CreateChecked(source)),
			_ => false,
		};
#endif
}

/// <summary>
///     The match type that compares by <see cref="EqualsMatchType.IsEqual{TActual, TExpected}" />.
/// </summary>
internal sealed class EqualsMatchType<TSubject> : ObjectMatchType<TSubject>
{
	internal static readonly EqualsMatchType<TSubject> Instance = new();
	private static readonly EqualsMatcher Matcher = new();

	private EqualsMatchType()
	{
	}

	/// <inheritdoc />
	public override MatchPhrase Phrase => MatchPhrase.EqualTo;

	/// <inheritdoc />
	public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
		=> Matcher;

	/// <summary>
	///     Whether the <paramref name="evaluator" /> compares by <see cref="EqualsMatchType.IsEqual{TActual, TExpected}" />.
	/// </summary>
	internal static bool IsUsedBy(IEqualityEvaluator<TSubject> evaluator)
		=> ReferenceEquals(evaluator, Matcher);

	/// <remarks>
	///     The comparison of the items of a collection is called directly, because the generic virtual call to
	///     <see cref="Match{TExpected}(TSubject, TExpected)" /> costs more than comparing most items.
	/// </remarks>
	private sealed class EqualsMatcher : ObjectMatcher<TSubject>, IEqualityEvaluator<TSubject>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected)
			=> new(MatchResult.From(EqualsMatchType.IsEqual(actual, expected)));

		/// <inheritdoc cref="IEqualityEvaluator{TSubject}.AreConsideredEqual{TExpected}(TSubject, TExpected)" />
		ValueTask<bool> IEqualityEvaluator<TSubject>.AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
			=> new(EqualsMatchType.IsEqual(actual, expected));
	}
}
