using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if !NET8_0_OR_GREATER
using System;
using aweXpect.SourceGenerators;
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
		expected.ThrowIfNull();
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
		expected.ThrowIfNull();
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
		expected.ThrowIfNull();
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
		expected.ThrowIfNull();
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
		expected.ThrowIfNull();
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
		expected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		unexpected.ThrowIfNull();
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
		: ConstraintResult.WithValue<TNumber>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory), Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOfWithNullableValuesCore<TNumber>(
		IThat<TNumber> subject,
		IEnumerable<TNumber?> expected,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		expected.ThrowIfNull(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NumberToleranceResult<TNumber, IThat<TNumber>> IsOneOfCore<TNumber>(
		IThat<TNumber> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		expected.ThrowIfNull(negated);
		return new NumberToleranceResult<TNumber, IThat<TNumber>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TNumber>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory), Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>>
		IsOneOfForNullableWithNullableValuesCore<TNumber>(
			IThat<TNumber?> subject,
			IEnumerable<TNumber?> expected,
			NumberTolerance<TNumber> options,
			bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		expected.ThrowIfNull(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraintWithNullable<TNumber>(it, grammars, expected, options)
					.InvertIf(negated)),
			subject,
			options);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Factory = typeof(NumberToleranceFactory),
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static NullableNumberToleranceResult<TNumber, IThat<TNumber?>> IsOneOfForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		expected.ThrowIfNull(negated);
		return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsOneOfConstraint<TNumber>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TNumber> expected,
		NumberTolerance<TNumber> options)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
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
		: ConstraintResult.WithValue<TNumber?>(it, grammars),
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			ValueFormatters.Format(Formatter, stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
