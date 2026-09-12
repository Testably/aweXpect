using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsEqualTo<TNumber>(
		this IThat<TNumber> source, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TNumber>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsEqualTo<TNumber>(
		this IThat<TNumber?> source, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<TNumber>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotEqualTo<TNumber>(
		this IThat<TNumber> source, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotEqualTo<TNumber>(
		this IThat<TNumber?> source, TNumber? unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	private sealed class IsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber>(it, grammars, expected is null),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = options.IsWithinTolerance(actual, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}

	private sealed class NullableIsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber?>(it, grammars, expected is null),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsWithinTolerance(actual, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
#else
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsEqualTo(
		this IThat<byte> source,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<byte, IThat<byte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<byte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsEqualTo(
		this IThat<sbyte> source,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<sbyte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsEqualTo(
		this IThat<short> source,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<short, IThat<short>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<short>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsEqualTo(
		this IThat<ushort> source,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<ushort>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsEqualTo(
		this IThat<int> source,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<int, IThat<int>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<int>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsEqualTo(
		this IThat<uint> source,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<uint, IThat<uint>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<uint>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsEqualTo(
		this IThat<long> source,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<long, IThat<long>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<long>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsEqualTo(
		this IThat<ulong> source,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<ulong>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsEqualTo(
		this IThat<float> source,
		float? expected)
	{
		NumberTolerance<float> options =
 new((a, e) => { checked { return float.IsNaN(a) || float.IsNaN(e) ? null : a > e ? a - e : e - a; } });
		return new NumberToleranceResult<float, IThat<float>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<float>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsEqualTo(
		this IThat<double> source,
		double? expected)
	{
		NumberTolerance<double> options =
 new((a, e) => { checked { return double.IsNaN(a) || double.IsNaN(e) ? null : a > e ? a - e : e - a; } });
		return new NumberToleranceResult<double, IThat<double>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<double>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsEqualTo(
		this IThat<decimal> source,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<decimal>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsEqualTo(
		this IThat<byte?> source,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<byte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsEqualTo(
		this IThat<sbyte?> source,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<sbyte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsEqualTo(
		this IThat<short?> source,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<short>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsEqualTo(
		this IThat<ushort?> source,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<ushort>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsEqualTo(
		this IThat<int?> source,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<int>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsEqualTo(
		this IThat<uint?> source,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<uint>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsEqualTo(
		this IThat<long?> source,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<long>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsEqualTo(
		this IThat<ulong?> source,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<ulong>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsEqualTo(
		this IThat<float?> source,
		float? expected)
	{
		NumberTolerance<float> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<float>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsEqualTo(
		this IThat<double?> source,
		double? expected)
	{
		NumberTolerance<double> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<double>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsEqualTo(
		this IThat<decimal?> source,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<decimal>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsNotEqualTo(
		this IThat<byte> source,
		byte? unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<byte, IThat<byte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<byte>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsNotEqualTo(
		this IThat<sbyte> source,
		sbyte? unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<sbyte>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsNotEqualTo(
		this IThat<short> source,
		short? unexpected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<short, IThat<short>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<short>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsNotEqualTo(
		this IThat<ushort> source,
		ushort? unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<ushort>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsNotEqualTo(
		this IThat<int> source,
		int? unexpected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<int, IThat<int>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<int>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsNotEqualTo(
		this IThat<uint> source,
		uint? unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<uint, IThat<uint>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<uint>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsNotEqualTo(
		this IThat<long> source,
		long? unexpected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<long, IThat<long>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<long>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsNotEqualTo(
		this IThat<ulong> source,
		ulong? unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<ulong>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsNotEqualTo(
		this IThat<float> source,
		float? unexpected)
	{
		NumberTolerance<float> options =
 new((a, e) => { checked { return float.IsNaN(a) || float.IsNaN(e) ? null : a > e ? a - e : e - a; } });
		return new NumberToleranceResult<float, IThat<float>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<float>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsNotEqualTo(
		this IThat<double> source,
		double? unexpected)
	{
		NumberTolerance<double> options =
 new((a, e) => { checked { return double.IsNaN(a) || double.IsNaN(e) ? null : a > e ? a - e : e - a; } });
		return new NumberToleranceResult<double, IThat<double>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<double>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsNotEqualTo(
		this IThat<decimal> source,
		decimal? unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<decimal>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsNotEqualTo(
		this IThat<byte?> source,
		byte? unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<byte>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsNotEqualTo(
		this IThat<sbyte?> source,
		sbyte? unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<sbyte>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsNotEqualTo(
		this IThat<short?> source,
		short? unexpected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<short>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsNotEqualTo(
		this IThat<ushort?> source,
		ushort? unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<ushort>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsNotEqualTo(
		this IThat<int?> source,
		int? unexpected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<int>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsNotEqualTo(
		this IThat<uint?> source,
		uint? unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<uint>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsNotEqualTo(
		this IThat<long?> source,
		long? unexpected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<long>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsNotEqualTo(
		this IThat<ulong?> source,
		ulong? unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<ulong>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsNotEqualTo(
		this IThat<float?> source,
		float? unexpected)
	{
		NumberTolerance<float> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<float>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsNotEqualTo(
		this IThat<double?> source,
		double? unexpected)
	{
		NumberTolerance<double> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<double>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsNotEqualTo(
		this IThat<decimal?> source,
		decimal? unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsEqualToConstraint<decimal>(it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	private sealed class IsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber>(it, grammars, expected is null),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = options.IsWithinTolerance(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}

	private sealed class NullableIsEqualToConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithEqualToValue<TNumber?>(it, grammars, expected is null),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsWithinTolerance(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
			AppendDifference(stringBuilder, Actual, expected, options);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
#endif
}
