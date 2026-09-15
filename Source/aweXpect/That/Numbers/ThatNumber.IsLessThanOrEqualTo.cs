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
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsLessThanOrEqualTo<TNumber>(
		this IThat<TNumber> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsLessThanOrEqualTo<TNumber>(
		this IThat<TNumber?> subject, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class IsLessThanOrEqualToConstraint<TNumber>(
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
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than or equal to ");
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
			stringBuilder.Append("is not less than or equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanOrEqualToConstraint<TNumber>(
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
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than or equal to ");
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
			stringBuilder.Append("is not less than or equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsLessThanOrEqualTo(
		this IThat<byte> subject,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsLessThanOrEqualTo(
		this IThat<sbyte> subject,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsLessThanOrEqualTo(
		this IThat<short> subject,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsLessThanOrEqualTo(
		this IThat<ushort> subject,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsLessThanOrEqualTo(
		this IThat<int> subject,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsLessThanOrEqualTo(
		this IThat<uint> subject,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsLessThanOrEqualTo(
		this IThat<long> subject,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsLessThanOrEqualTo(
		this IThat<ulong> subject,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsLessThanOrEqualTo(
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
				new IsLessThanOrEqualToConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsLessThanOrEqualTo(
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
				new IsLessThanOrEqualToConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsLessThanOrEqualTo(
		this IThat<decimal> subject,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLessThanOrEqualToConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsLessThanOrEqualTo(
		this IThat<byte?> subject,
		byte? expected)
	{
		NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsLessThanOrEqualTo(
		this IThat<sbyte?> subject,
		sbyte? expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<short, IThat<short?>> IsLessThanOrEqualTo(
		this IThat<short?> subject,
		short? expected)
	{
		NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsLessThanOrEqualTo(
		this IThat<ushort?> subject,
		ushort? expected)
	{
		NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<int, IThat<int?>> IsLessThanOrEqualTo(
		this IThat<int?> subject,
		int? expected)
	{
		NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsLessThanOrEqualTo(
		this IThat<uint?> subject,
		uint? expected)
	{
		NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<long, IThat<long?>> IsLessThanOrEqualTo(
		this IThat<long?> subject,
		long? expected)
	{
		NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsLessThanOrEqualTo(
		this IThat<ulong?> subject,
		ulong? expected)
	{
		NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<float, IThat<float?>> IsLessThanOrEqualTo(
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
				new NullableIsLessThanOrEqualToConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<double, IThat<double?>> IsLessThanOrEqualTo(
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
				new NullableIsLessThanOrEqualToConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsLessThanOrEqualTo(
		this IThat<decimal?> subject,
		decimal? expected)
	{
		NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsLessThanOrEqualToConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class IsLessThanOrEqualToConstraint<TNumber>(
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
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than or equal to ");
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
			stringBuilder.Append("is not less than or equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsLessThanOrEqualToConstraint<TNumber>(
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
			Outcome = options.IsLessThanOrEqualTo(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is less than or equal to ");
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
			stringBuilder.Append("is not less than or equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
