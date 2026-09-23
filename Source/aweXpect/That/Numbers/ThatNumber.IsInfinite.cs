using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsInfinite = "is infinite";
	private const string ExpectIsNotInfinite = "is not infinite";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is seen as infinite.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsInfinite<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsInfiniteConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as infinite.
	/// </summary>
	/// <remarks>
	///     <see langword="null" /> is not treated as infinite.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsInfinite<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsInfiniteConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as infinite.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotInfinite<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsInfiniteConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as infinite.
	/// </summary>
	/// <remarks>
	///     <see langword="null" /> is treated as not infinite.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotInfinite<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsInfiniteConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsInfiniteConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = TNumber.IsInfinity(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsInfinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotInfinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsInfiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && TNumber.IsInfinity(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsInfinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotInfinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsInfiniteSummary = "Verifies that the subject is seen as infinite.";
	private const string IsNotInfiniteSummary = "Verifies that the subject is not seen as infinite.";

	[CreateCollectionExpectation("Is{Not}Infinite", Factory = typeof(FloatingPointNumberFactory),
		Summary = IsInfiniteSummary, NegatedSummary = IsNotInfiniteSummary)]
	internal static AndOrResult<TNumber, IThat<TNumber>> IsInfiniteCore<TNumber>(
		IThat<TNumber> subject,
		FloatingPointTraits<TNumber> traits,
		bool negated)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsInfiniteConstraint<TNumber>(it, grammars, traits.IsInfinity).InvertIf(negated)),
			subject);

	[CreateCollectionExpectation("Is{Not}Infinite", Factory = typeof(FloatingPointNumberFactory),
		GuaranteesNotNull = true, Summary = IsInfiniteSummary, NegatedSummary = IsNotInfiniteSummary,
		Remarks = "<see langword=\"null\" /> is not treated as infinite.",
		NegatedRemarks = "<see langword=\"null\" /> is treated as not infinite.")]
	internal static AndOrResult<TNumber?, IThat<TNumber?>> IsInfiniteForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		FloatingPointTraits<TNumber> traits,
		bool negated)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsInfiniteConstraint<TNumber>(it, grammars, traits.IsInfinity).InvertIf(negated)),
			subject);

	private sealed class IsInfiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isInfinity)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = isInfinity(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsInfinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotInfinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsInfiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isInfinity)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && isInfinity(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsInfinite);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(ExpectIsNotInfinite);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
