using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatNumber
{
	private const string ExpectIsFinite = "is finite";
	private const string ExpectIsNotFinite = "is not finite";
	private const string ExpectAreFinite = "are finite";
	private const string ExpectAreNotFinite = "are not finite";

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is seen as finite.
	/// </summary>
	/// <remarks>
	///     Finite means neither infinity nor not a number (NaN).
	/// </remarks>
	public static AndOrResult<TNumber, IThat<TNumber>> IsFinite<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFiniteConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is seen as finite.
	/// </summary>
	/// <remarks>
	///     Finite means neither infinity nor not a number (NaN) nor <see langword="null" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsFinite<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as finite.
	/// </summary>
	/// <remarks>
	///     Not finite means either infinity or not a number (NaN).
	/// </remarks>
	public static AndOrResult<TNumber, IThat<TNumber>> IsNotFinite<TNumber>(this IThat<TNumber> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFiniteConstraint<TNumber>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not seen as finite.
	/// </summary>
	/// <remarks>
	///     Not finite means either infinity or not a number (NaN). A <see langword="null" /> subject fails.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNumber?, IThat<TNumber?>> IsNotFinite<TNumber>(this IThat<TNumber?> subject)
		where TNumber : struct, IFloatingPoint<TNumber>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars).Invert()),
			subject);

	private sealed class IsFiniteConstraint<TNumber>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = !TNumber.IsInfinity(actual) && !TNumber.IsNaN(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsFinite, ExpectAreFinite));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotFinite, ExpectAreNotFinite));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsFiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct, IFloatingPoint<TNumber>
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && !TNumber.IsInfinity(actual.Value) && !TNumber.IsNaN(actual.Value)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsFinite, ExpectAreFinite));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotFinite, ExpectAreNotFinite));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	private const string IsFiniteSummary = "Verifies that the subject is seen as finite.";
	private const string IsNotFiniteSummary = "Verifies that the subject is not seen as finite.";
	private const string FiniteRemarks = "Finite means neither infinity nor not a number (NaN).";
	private const string NotFiniteRemarks = "Not finite means either infinity or not a number (NaN).";

	[CreateCollectionExpectation("Is{Not}Finite", Factory = typeof(FloatingPointNumberFactory),
		Summary = IsFiniteSummary, NegatedSummary = IsNotFiniteSummary,
		Remarks = FiniteRemarks, NegatedRemarks = NotFiniteRemarks)]
	internal static AndOrResult<TNumber, IThat<TNumber>> IsFiniteCore<TNumber>(
		IThat<TNumber> subject,
		FloatingPointTraits<TNumber> traits,
		bool negated)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFiniteConstraint<TNumber>(it, grammars, traits.IsFinite).InvertIf(negated)),
			subject);

	[CreateCollectionExpectation("IsFinite", Factory = typeof(FloatingPointNumberFactory), GuaranteesNotNull = true,
		Summary = IsFiniteSummary,
		Remarks = "Finite means neither infinity nor not a number (NaN) nor <see langword=\"null\" />.")]
	internal static AndOrResult<TNumber, IThat<TNumber?>> IsFiniteForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		FloatingPointTraits<TNumber> traits)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars, traits.IsFinite)),
			subject);

	[CreateCollectionExpectation("IsNotFinite", Factory = typeof(FloatingPointNumberFactory), GuaranteesNotNull = true,
		Summary = IsNotFiniteSummary,
		Remarks = "Not finite means either infinity or not a number (NaN). A <see langword=\"null\" /> subject fails.")]
	internal static AndOrResult<TNumber?, IThat<TNumber?>> IsNotFiniteForNullableCore<TNumber>(
		IThat<TNumber?> subject,
		FloatingPointTraits<TNumber> traits)
		where TNumber : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new NullableIsFiniteConstraint<TNumber>(it, grammars, traits.IsFinite).Invert()),
			subject);

	private sealed class IsFiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isFinite)
		: ConstraintResult.WithValue<TNumber>(it, grammars),
			IValueConstraint<TNumber>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = isFinite(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsFinite, ExpectAreFinite));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotFinite, ExpectAreNotFinite));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsFiniteConstraint<TNumber>(
		string it,
		ExpectationGrammars grammars,
		Func<TNumber, bool> isFinite)
		: ConstraintResult.WithNotNullValue<TNumber?>(it, grammars),
			IValueConstraint<TNumber?>
		where TNumber : struct
	{
		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = actual is not null && isFinite(actual.Value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsFinite, ExpectAreFinite));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb(ExpectIsNotFinite, ExpectAreNotFinite));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
