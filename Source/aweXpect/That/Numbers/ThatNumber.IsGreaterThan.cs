using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if !NET8_0_OR_GREATER
using System;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsGreaterThan<TNumber>(
		this IThat<TNumber> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsGreaterThan<TNumber>(
		this IThat<TNumber?> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class IsGreaterThanConstraint<TNumber>(
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
			Outcome = options.IsGreaterThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsGreaterThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsGreaterThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsGreaterThan(
		this IThat<byte> subject,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsGreaterThan(
		this IThat<sbyte> subject,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsGreaterThan(
		this IThat<short> subject,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsGreaterThan(
		this IThat<ushort> subject,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsGreaterThan(
		this IThat<int> subject,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsGreaterThan(
		this IThat<uint> subject,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsGreaterThan(
		this IThat<long> subject,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsGreaterThan(
		this IThat<ulong> subject,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsGreaterThan(
		this IThat<float> subject,
		float? expected)
	{
		NumberTolerance<float> options = new((a, e) =>
		{
			if (float.IsNaN(a) || float.IsNaN(e))
			{
				return null;
			}

			return a > e ? a - e : e - a;
		});
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsGreaterThan(
		this IThat<double> subject,
		double? expected)
	{
		NumberTolerance<double> options = new((a, e) =>
		{
			if (double.IsNaN(a) || double.IsNaN(e))
			{
				return null;
			}

			return a > e ? a - e : e - a;
		});
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsGreaterThan(
		this IThat<decimal> subject,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsGreaterThanConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsGreaterThan(
		this IThat<byte?> subject,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsGreaterThan(
		this IThat<sbyte?> subject,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<short, IThat<short?>> IsGreaterThan(
		this IThat<short?> subject,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsGreaterThan(
		this IThat<ushort?> subject,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<int, IThat<int?>> IsGreaterThan(
		this IThat<int?> subject,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsGreaterThan(
		this IThat<uint?> subject,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<long, IThat<long?>> IsGreaterThan(
		this IThat<long?> subject,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsGreaterThan(
		this IThat<ulong?> subject,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<float, IThat<float?>> IsGreaterThan(
		this IThat<float?> subject,
		float? expected)
	{
		NumberTolerance<float> options = new((a, e) =>
		{
			if (float.IsNaN(a) || float.IsNaN(e))
			{
				return null;
			}

			return a > e ? a - e : e - a;
		});
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<double, IThat<double?>> IsGreaterThan(
		this IThat<double?> subject,
		double? expected)
	{
		NumberTolerance<double> options = new((a, e) =>
		{
			if (double.IsNaN(a) || double.IsNaN(e))
			{
				return null;
			}

			return a > e ? a - e : e - a;
		});
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is greater than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsGreaterThan(
		this IThat<decimal?> subject,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsGreaterThanConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class IsGreaterThanConstraint<TNumber>(
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
			Outcome = options.IsGreaterThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsGreaterThanConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		TNumber? expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = options.IsGreaterThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not greater than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
