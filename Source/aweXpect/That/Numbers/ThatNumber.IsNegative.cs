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
	private const string ExpectIsNegative = "is negative";
	private const string ExpectIsNotNegative = "is not negative";
	private const string ExpectAreNegative = "are negative";
	private const string ExpectAreNotNegative = "are not negative";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	/// <remarks>
	///     Neither zero nor <c>NaN</c> is positive or negative, so both fail <c>IsPositive</c> and <c>IsNegative</c>
	///     and satisfy <c>IsNotPositive</c> and <c>IsNotNegative</c>.
	/// </remarks>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNegative<TNumber>(
		this IThat<TNumber> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is negative.
	/// </summary>
	/// <remarks>
	///     Neither zero nor <c>NaN</c> is positive or negative, so both fail <c>IsPositive</c> and <c>IsNegative</c>
	///     and satisfy <c>IsNotPositive</c> and <c>IsNotNegative</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNegative<TNumber>(
		this IThat<TNumber?> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not negative.
	/// </summary>
	/// <remarks>
	///     Neither zero nor <c>NaN</c> is positive or negative, so both fail <c>IsPositive</c> and <c>IsNegative</c>
	///     and satisfy <c>IsNotPositive</c> and <c>IsNotNegative</c>.
	/// </remarks>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotNegative<TNumber>(
		this IThat<TNumber> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not negative.
	/// </summary>
	/// <remarks>
	///     Neither zero nor <c>NaN</c> is positive or negative, so both fail <c>IsPositive</c> and <c>IsNegative</c>
	///     and satisfy <c>IsNotPositive</c> and <c>IsNotNegative</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotNegative<TNumber>(
		this IThat<TNumber?> subject)
		where TNumber : struct, INumber<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsNegativeConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = actual < default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNegative, ExpectAreNegative));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNegative, ExpectAreNotNegative));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNegativeConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual < default(TNumber)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNegative, ExpectAreNegative));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNegative, ExpectAreNotNegative));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsNegativeSummary = "Verifies that the subject is negative.";
	private const string IsNotNegativeSummary = "Verifies that the subject is not negative.";

	private const string IsNegativeRemarks =
		"Neither zero nor <c>NaN</c> is positive or negative, so both fail <c>IsPositive</c> and <c>IsNegative</c>\n" +
		"and satisfy <c>IsNotPositive</c> and <c>IsNotNegative</c>.";

	[CreateCollectionExpectation("Is{Not}Negative", Factory = typeof(SignedNumberFactory),
		Summary = IsNegativeSummary, NegatedSummary = IsNotNegativeSummary, Remarks = IsNegativeRemarks)]
	internal static AndOrResult<TNumber, IThat<TNumber>> IsNegativeCore<TNumber>(
		IThat<TNumber> subject,
		NumberSign<TNumber> sign,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNegativeConstraint<TNumber>(it, grammars, sign.IsNegative).InvertIf(negated)),
			subject);

	[CreateCollectionExpectation("Is{Not}Negative", Factory = typeof(SignedNumberFactory), GuaranteesNotNull = true,
		Summary = IsNegativeSummary, NegatedSummary = IsNotNegativeSummary, Remarks = IsNegativeRemarks)]
	internal static AndOrResult<TNumber?, IThat<TNumber?>> IsNegativeForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		NumberSign<TNumber> sign,
		bool negated)
		where TNumber : struct, IComparable<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsNegativeConstraint<TNumber>(it, grammars, sign.IsNegative).InvertIf(negated)),
			subject);

	private sealed class IsNegativeConstraint<TNumber>(
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
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNegative, ExpectAreNegative));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNegative, ExpectAreNotNegative));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsNegativeConstraint<TNumber>(
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
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNegative, ExpectAreNegative));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotNegative, ExpectAreNotNegative));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
