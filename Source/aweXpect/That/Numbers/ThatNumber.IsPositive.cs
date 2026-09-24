using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;
#if !NET8_0_OR_GREATER
using System;
#endif

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsPositive = "is positive";
	private const string ExpectIsNotPositive = "is not positive";
	private const string ExpectArePositive = "are positive";
	private const string ExpectAreNotPositive = "are not positive";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsPositive<TNumber>(
		this IThat<TNumber> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsPositive<TNumber>(
		this IThat<TNumber?> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not positive.
	/// </summary>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotPositive<TNumber>(
		this IThat<TNumber> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not positive.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotPositive<TNumber>(
		this IThat<TNumber?> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsPositiveConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = actual > default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsPositive, ExpectArePositive));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotPositive, ExpectAreNotPositive));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual > default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsPositive, ExpectArePositive));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotPositive, ExpectAreNotPositive));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsPositiveSummary = "Verifies that the subject is positive.";
	private const string IsNotPositiveSummary = "Verifies that the subject is not positive.";

	[CreateCollectionExpectation("Is{Not}Positive", Factory = typeof(SignedNumberFactory),
		Summary = IsPositiveSummary, NegatedSummary = IsNotPositiveSummary)]
	internal static AndOrResult<TNumber, IThat<TNumber>> IsPositiveCore<TNumber>(
		IThat<TNumber> subject,
		NumberSign<TNumber> sign,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsPositiveConstraint<TNumber>(it, grammars, sign.IsPositive).InvertIf(negated)),
			subject);

	[CreateCollectionExpectation("Is{Not}Positive", Factory = typeof(SignedNumberFactory), GuaranteesNotNull = true,
		Summary = IsPositiveSummary, NegatedSummary = IsNotPositiveSummary)]
	internal static AndOrResult<TNumber?, IThat<TNumber?>> IsPositiveForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		NumberSign<TNumber> sign,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsPositiveConstraint<TNumber>(it, grammars, sign.IsPositive).InvertIf(negated)),
			subject);

	private sealed class IsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> predicate)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = predicate(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsPositive, ExpectArePositive));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotPositive, ExpectAreNotPositive));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsPositiveConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> predicate)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && predicate(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsPositive, ExpectArePositive));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotPositive, ExpectAreNotPositive));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
