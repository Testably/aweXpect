using System.Collections.Generic;
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
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		params TNumber?[] expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		params TNumber?[] expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber> expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber> expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber?> expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber?> expected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		params TNumber?[] unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		params TNumber?[] unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber> unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber> unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<TNumber, IThat<TNumber>> IsNotOneOf<TNumber>(
		this IThat<TNumber> subject,
		IEnumerable<TNumber?> unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsNotOneOf<TNumber>(
		this IThat<TNumber?> subject,
		IEnumerable<TNumber?> unexpected)
		where TNumber : struct, INumber<TNumber>
	{
		NumberTolerance<TNumber> options = new(CalculateDifference);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber? value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber? value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsOneOf(
		this IThat<byte> subject,
		params byte?[] expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsOneOf(
		this IThat<byte> subject,
		IEnumerable<byte?> expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsOneOf(
		this IThat<byte> subject,
		IEnumerable<byte> expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsOneOf(
		this IThat<sbyte> subject,
		params sbyte?[] expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsOneOf(
		this IThat<sbyte> subject,
		IEnumerable<sbyte?> expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsOneOf(
		this IThat<sbyte> subject,
		IEnumerable<sbyte> expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsOneOf(
		this IThat<short> subject,
		params short?[] expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsOneOf(
		this IThat<short> subject,
		IEnumerable<short?> expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsOneOf(
		this IThat<short> subject,
		IEnumerable<short> expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsOneOf(
		this IThat<ushort> subject,
		params ushort?[] expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsOneOf(
		this IThat<ushort> subject,
		IEnumerable<ushort?> expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsOneOf(
		this IThat<ushort> subject,
		IEnumerable<ushort> expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsOneOf(
		this IThat<int> subject,
		params int?[] expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsOneOf(
		this IThat<int> subject,
		IEnumerable<int?> expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsOneOf(
		this IThat<int> subject,
		IEnumerable<int> expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsOneOf(
		this IThat<uint> subject,
		params uint?[] expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsOneOf(
		this IThat<uint> subject,
		IEnumerable<uint?> expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsOneOf(
		this IThat<uint> subject,
		IEnumerable<uint> expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsOneOf(
		this IThat<long> subject,
		params long?[] expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsOneOf(
		this IThat<long> subject,
		IEnumerable<long?> expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsOneOf(
		this IThat<long> subject,
		IEnumerable<long> expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsOneOf(
		this IThat<ulong> subject,
		params ulong?[] expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsOneOf(
		this IThat<ulong> subject,
		IEnumerable<ulong?> expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsOneOf(
		this IThat<ulong> subject,
		IEnumerable<ulong> expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsOneOf(
		this IThat<float> subject,
		params float?[] expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsOneOf(
		this IThat<float> subject,
		IEnumerable<float?> expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsOneOf(
		this IThat<float> subject,
		IEnumerable<float> expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsOneOf(
		this IThat<double> subject,
		params double?[] expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsOneOf(
		this IThat<double> subject,
		IEnumerable<double?> expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsOneOf(
		this IThat<double> subject,
		IEnumerable<double> expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsOneOf(
		this IThat<decimal> subject,
		params decimal?[] expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsOneOf(
		this IThat<decimal> subject,
		IEnumerable<decimal?> expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsOneOf(
		this IThat<decimal> subject,
		IEnumerable<decimal> expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsOneOf(
		this IThat<byte?> subject,
		params byte?[] expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsOneOf(
		this IThat<byte?> subject,
		IEnumerable<byte?> expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsOneOf(
		this IThat<byte?> subject,
		IEnumerable<byte> expected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<byte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsOneOf(
		this IThat<sbyte?> subject,
		params sbyte?[] expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsOneOf(
		this IThat<sbyte?> subject,
		IEnumerable<sbyte?> expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsOneOf(
		this IThat<sbyte?> subject,
		IEnumerable<sbyte> expected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<sbyte>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsOneOf(
		this IThat<short?> subject,
		params short?[] expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsOneOf(
		this IThat<short?> subject,
		IEnumerable<short?> expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsOneOf(
		this IThat<short?> subject,
		IEnumerable<short> expected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<short>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsOneOf(
		this IThat<ushort?> subject,
		params ushort?[] expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsOneOf(
		this IThat<ushort?> subject,
		IEnumerable<ushort?> expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsOneOf(
		this IThat<ushort?> subject,
		IEnumerable<ushort> expected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<ushort>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsOneOf(
		this IThat<int?> subject,
		params int?[] expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsOneOf(
		this IThat<int?> subject,
		IEnumerable<int?> expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsOneOf(
		this IThat<int?> subject,
		IEnumerable<int> expected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<int>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsOneOf(
		this IThat<uint?> subject,
		params uint?[] expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsOneOf(
		this IThat<uint?> subject,
		IEnumerable<uint?> expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsOneOf(
		this IThat<uint?> subject,
		IEnumerable<uint> expected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<uint>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsOneOf(
		this IThat<long?> subject,
		params long?[] expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsOneOf(
		this IThat<long?> subject,
		IEnumerable<long?> expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsOneOf(
		this IThat<long?> subject,
		IEnumerable<long> expected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<long>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsOneOf(
		this IThat<ulong?> subject,
		params ulong?[] expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsOneOf(
		this IThat<ulong?> subject,
		IEnumerable<ulong?> expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsOneOf(
		this IThat<ulong?> subject,
		IEnumerable<ulong> expected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<ulong>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsOneOf(
		this IThat<float?> subject,
		params float?[] expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsOneOf(
		this IThat<float?> subject,
		IEnumerable<float?> expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsOneOf(
		this IThat<float?> subject,
		IEnumerable<float> expected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<float>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsOneOf(
		this IThat<double?> subject,
		params double?[] expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsOneOf(
		this IThat<double?> subject,
		IEnumerable<double?> expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsOneOf(
		this IThat<double?> subject,
		IEnumerable<double> expected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<double>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsOneOf(
		this IThat<decimal?> subject,
		params decimal?[] expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsOneOf(
		this IThat<decimal?> subject,
		IEnumerable<decimal?> expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsOneOf(
		this IThat<decimal?> subject,
		IEnumerable<decimal> expected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<decimal>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsNotOneOf(
		this IThat<byte> subject,
		params byte?[] unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsNotOneOf(
		this IThat<byte> subject,
		IEnumerable<byte?> unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<byte, IThat<byte>> IsNotOneOf(
		this IThat<byte> subject,
		IEnumerable<byte> unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NumberToleranceResult<byte, IThat<byte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsNotOneOf(
		this IThat<sbyte> subject,
		params sbyte?[] unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsNotOneOf(
		this IThat<sbyte> subject,
		IEnumerable<sbyte?> unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<sbyte, IThat<sbyte>> IsNotOneOf(
		this IThat<sbyte> subject,
		IEnumerable<sbyte> unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NumberToleranceResult<sbyte, IThat<sbyte>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsNotOneOf(
		this IThat<short> subject,
		params short?[] unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsNotOneOf(
		this IThat<short> subject,
		IEnumerable<short?> unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<short, IThat<short>> IsNotOneOf(
		this IThat<short> subject,
		IEnumerable<short> unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NumberToleranceResult<short, IThat<short>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsNotOneOf(
		this IThat<ushort> subject,
		params ushort?[] unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsNotOneOf(
		this IThat<ushort> subject,
		IEnumerable<ushort?> unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ushort, IThat<ushort>> IsNotOneOf(
		this IThat<ushort> subject,
		IEnumerable<ushort> unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NumberToleranceResult<ushort, IThat<ushort>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsNotOneOf(
		this IThat<int> subject,
		params int?[] unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsNotOneOf(
		this IThat<int> subject,
		IEnumerable<int?> unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<int, IThat<int>> IsNotOneOf(
		this IThat<int> subject,
		IEnumerable<int> unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<int, IThat<int>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsNotOneOf(
		this IThat<uint> subject,
		params uint?[] unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsNotOneOf(
		this IThat<uint> subject,
		IEnumerable<uint?> unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<uint, IThat<uint>> IsNotOneOf(
		this IThat<uint> subject,
		IEnumerable<uint> unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<uint, IThat<uint>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsNotOneOf(
		this IThat<long> subject,
		params long?[] unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsNotOneOf(
		this IThat<long> subject,
		IEnumerable<long?> unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<long, IThat<long>> IsNotOneOf(
		this IThat<long> subject,
		IEnumerable<long> unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<long, IThat<long>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsNotOneOf(
		this IThat<ulong> subject,
		params ulong?[] unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsNotOneOf(
		this IThat<ulong> subject,
		IEnumerable<ulong?> unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<ulong, IThat<ulong>> IsNotOneOf(
		this IThat<ulong> subject,
		IEnumerable<ulong> unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NumberToleranceResult<ulong, IThat<ulong>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsNotOneOf(
		this IThat<float> subject,
		params float?[] unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsNotOneOf(
		this IThat<float> subject,
		IEnumerable<float?> unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<float, IThat<float>> IsNotOneOf(
		this IThat<float> subject,
		IEnumerable<float> unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<float, IThat<float>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsNotOneOf(
		this IThat<double> subject,
		params double?[] unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsNotOneOf(
		this IThat<double> subject,
		IEnumerable<double?> unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<double, IThat<double>> IsNotOneOf(
		this IThat<double> subject,
		IEnumerable<double> unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NumberToleranceResult<double, IThat<double>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsNotOneOf(
		this IThat<decimal> subject,
		params decimal?[] unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsNotOneOf(
		this IThat<decimal> subject,
		IEnumerable<decimal?> unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NumberToleranceResult<decimal, IThat<decimal>> IsNotOneOf(
		this IThat<decimal> subject,
		IEnumerable<decimal> unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NumberToleranceResult<decimal, IThat<decimal>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsNotOneOf(
		this IThat<byte?> subject,
		params byte?[] unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsNotOneOf(
		this IThat<byte?> subject,
		IEnumerable<byte?> unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<byte, IThat<byte?>> IsNotOneOf(
		this IThat<byte?> subject,
		IEnumerable<byte> unexpected)
	{
		NumberTolerance<byte> options = new((a, e) => (byte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<byte, IThat<byte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<byte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsNotOneOf(
		this IThat<sbyte?> subject,
		params sbyte?[] unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsNotOneOf(
		this IThat<sbyte?> subject,
		IEnumerable<sbyte?> unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<sbyte, IThat<sbyte?>> IsNotOneOf(
		this IThat<sbyte?> subject,
		IEnumerable<sbyte> unexpected)
	{
		NumberTolerance<sbyte> options = new((a, e) => (sbyte)Math.Abs(a - e));
		return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<sbyte>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsNotOneOf(
		this IThat<short?> subject,
		params short?[] unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsNotOneOf(
		this IThat<short?> subject,
		IEnumerable<short?> unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<short, IThat<short?>> IsNotOneOf(
		this IThat<short?> subject,
		IEnumerable<short> unexpected)
	{
		NumberTolerance<short> options = new((a, e) => (short)Math.Abs(a - e));
		return new NullableNumberToleranceResult<short, IThat<short?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<short>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsNotOneOf(
		this IThat<ushort?> subject,
		params ushort?[] unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsNotOneOf(
		this IThat<ushort?> subject,
		IEnumerable<ushort?> unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ushort, IThat<ushort?>> IsNotOneOf(
		this IThat<ushort?> subject,
		IEnumerable<ushort> unexpected)
	{
		NumberTolerance<ushort> options = new((a, e) => (ushort)Math.Abs(a - e));
		return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<ushort>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsNotOneOf(
		this IThat<int?> subject,
		params int?[] unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsNotOneOf(
		this IThat<int?> subject,
		IEnumerable<int?> unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<int, IThat<int?>> IsNotOneOf(
		this IThat<int?> subject,
		IEnumerable<int> unexpected)
	{
		NumberTolerance<int> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<int, IThat<int?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<int>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsNotOneOf(
		this IThat<uint?> subject,
		params uint?[] unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsNotOneOf(
		this IThat<uint?> subject,
		IEnumerable<uint?> unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<uint, IThat<uint?>> IsNotOneOf(
		this IThat<uint?> subject,
		IEnumerable<uint> unexpected)
	{
		NumberTolerance<uint> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<uint, IThat<uint?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<uint>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsNotOneOf(
		this IThat<long?> subject,
		params long?[] unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsNotOneOf(
		this IThat<long?> subject,
		IEnumerable<long?> unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<long, IThat<long?>> IsNotOneOf(
		this IThat<long?> subject,
		IEnumerable<long> unexpected)
	{
		NumberTolerance<long> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<long, IThat<long?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<long>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsNotOneOf(
		this IThat<ulong?> subject,
		params ulong?[] unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsNotOneOf(
		this IThat<ulong?> subject,
		IEnumerable<ulong?> unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<ulong, IThat<ulong?>> IsNotOneOf(
		this IThat<ulong?> subject,
		IEnumerable<ulong> unexpected)
	{
		NumberTolerance<ulong> options = new((a, e) => a > e ? a - e : e - a);
		return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<ulong>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsNotOneOf(
		this IThat<float?> subject,
		params float?[] unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsNotOneOf(
		this IThat<float?> subject,
		IEnumerable<float?> unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<float, IThat<float?>> IsNotOneOf(
		this IThat<float?> subject,
		IEnumerable<float> unexpected)
	{
		NumberTolerance<float> options = new((a, e) => float.IsNaN(a) || float.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<float, IThat<float?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<float>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsNotOneOf(
		this IThat<double?> subject,
		params double?[] unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsNotOneOf(
		this IThat<double?> subject,
		IEnumerable<double?> unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<double, IThat<double?>> IsNotOneOf(
		this IThat<double?> subject,
		IEnumerable<double> unexpected)
	{
		NumberTolerance<double> options = new((a, e) => double.IsNaN(a) || double.IsNaN(e) ? null : Math.Abs(a - e));
		return new NullableNumberToleranceResult<double, IThat<double?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<double>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsNotOneOf(
		this IThat<decimal?> subject,
		params decimal?[] unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsNotOneOf(
		this IThat<decimal?> subject,
		IEnumerable<decimal?> unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static NullableNumberToleranceResult<decimal, IThat<decimal?>> IsNotOneOf(
		this IThat<decimal?> subject,
		IEnumerable<decimal> unexpected)
	{
		NumberTolerance<decimal> options = new((a, e) => Math.Abs(a - e));
		return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<decimal>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class IsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber? value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsOneOfConstraintWithNullable<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber?>(grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TNumber? value in expected)
			{
				hasValues = true;
				if (options.IsWithinTolerance(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
