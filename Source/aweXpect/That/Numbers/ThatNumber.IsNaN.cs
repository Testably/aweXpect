using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsNaN = "is NaN";
	private const string ExpectIsNotNaN = "is not NaN";
	private const string ExpectAreNaN = "are NaN";
	private const string ExpectAreNotNaN = "are not NaN";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is seen as not a number.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNaN<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNaNConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as not a number.
	/// </summary>
	/// <remarks>
	///     <see langword="null" /> is not treated as NaN.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNaN<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNaNConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotNaN<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNaNConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as not a number.
	/// </summary>
	/// <remarks>
	///     <see langword="null" /> is neither treated as NaN nor as not NaN, so it fails.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotNaN<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNaNConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsNaNConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = TNumber.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNaN, ExpectAreNaN));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNaN, ExpectAreNotNaN));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNaNConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && TNumber.IsNaN(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNaN, ExpectAreNaN));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNaN, ExpectAreNotNaN));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsNaNSummary = "Verifies that the subject is seen as not a number.";
	private const string IsNotNaNSummary = "Verifies that the subject is not seen as not a number.";

	[CreateCollectionExpectation("Is{Not}NaN", Factory = typeof(FloatingPointNumberFactory),
		Summary = IsNaNSummary, NegatedSummary = IsNotNaNSummary)]
	internal static AndOrResult<TNumber, IThat<TNumber>> IsNaNCore<TNumber>(
		IThat<TNumber> subject,
		FloatingPointTraits<TNumber> traits,
		bool negated)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNaNConstraint<TNumber>(it, grammars, traits.IsNaN).InvertIf(negated)),
			subject);

	[CreateCollectionExpectation("Is{Not}NaN", Factory = typeof(FloatingPointNumberFactory), GuaranteesNotNull = true,
		Summary = IsNaNSummary, NegatedSummary = IsNotNaNSummary,
		Remarks = "<see langword=\"null\" /> is not treated as NaN.",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as NaN nor as not NaN, so it fails.")]
	internal static AndOrResult<TNumber?, IThat<TNumber?>> IsNaNForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		FloatingPointTraits<TNumber> traits,
		bool negated)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNaNConstraint<TNumber>(it, grammars, traits.IsNaN).InvertIf(negated)),
			subject);

	private sealed class IsNaNConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isNaN)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = isNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNaN, ExpectAreNaN));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNaN, ExpectAreNotNaN));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNaNConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isNaN)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && isNaN(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNaN, ExpectAreNaN));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNaN, ExpectAreNotNaN));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
