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
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThan<TNumber>(
		this IThat<TNumber> source, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<TNumber>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThan<TNumber>(
		this IThat<TNumber?> source, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<TNumber>(it, grammars, expected, options)),
			source,
			options);
	}

	private sealed class IsLessThanConstraint<TNumber>(
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
			Outcome = options.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than ");
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
			stringBuilder.Append("is not less than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanConstraint<TNumber>(
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
			Outcome = options.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than ");
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
			stringBuilder.Append("is not less than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<byte, IThat<byte>> IsLessThan(
		this IThat<byte> source,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<byte, IThat<byte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<byte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsLessThan(
		this IThat<sbyte> source,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<sbyte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<short, IThat<short>> IsLessThan(
		this IThat<short> source,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<short, IThat<short>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<short>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<ushort, IThat<ushort>> IsLessThan(
		this IThat<ushort> source,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<ushort>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<int, IThat<int>> IsLessThan(
		this IThat<int> source,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<int, IThat<int>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<int>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<uint, IThat<uint>> IsLessThan(
		this IThat<uint> source,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<uint, IThat<uint>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<uint>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<long, IThat<long>> IsLessThan(
		this IThat<long> source,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<long, IThat<long>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<long>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<ulong, IThat<ulong>> IsLessThan(
		this IThat<ulong> source,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<ulong>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<float, IThat<float>> IsLessThan(
		this IThat<float> source,
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
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<float>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<double, IThat<double>> IsLessThan(
		this IThat<double> source,
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
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<double>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NumberToleranceResult<decimal, IThat<decimal>> IsLessThan(
		this IThat<decimal> source,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanConstraint<decimal>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsLessThan(
		this IThat<byte?> source,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<byte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsLessThan(
		this IThat<sbyte?> source,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<sbyte>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<short, IThat<short?>> IsLessThan(
		this IThat<short?> source,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<short>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsLessThan(
		this IThat<ushort?> source,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<ushort>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<int, IThat<int?>> IsLessThan(
		this IThat<int?> source,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<int>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsLessThan(
		this IThat<uint?> source,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<uint>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<long, IThat<long?>> IsLessThan(
		this IThat<long?> source,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<long>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsLessThan(
		this IThat<ulong?> source,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<ulong>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<float, IThat<float?>> IsLessThan(
		this IThat<float?> source,
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
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<float>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<double, IThat<double?>> IsLessThan(
		this IThat<double?> source,
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
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<double>(it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsLessThan(
		this IThat<decimal?> source,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanConstraint<decimal>(it, grammars, expected, options)),
			source,
			options);
	}

	private sealed class IsLessThanConstraint<TNumber>(
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
			Outcome = options.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than ");
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
			stringBuilder.Append("is not less than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanConstraint<TNumber>(
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
			Outcome = options.IsLessThan(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than ");
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
			stringBuilder.Append("is not less than ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
