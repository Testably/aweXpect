using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatEnum
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static AndOrResult<TEnum, IThat<TEnum>> IsOneOfCore<TEnum>(
		IThat<TEnum> subject,
		IEnumerable<TEnum?> expected,
		string? expectedExpression,
		bool negated)
		where TEnum : struct, Enum
	{
		IEnumerable<TEnum?> expectedValues = expected.ToNonEmptyValues(negated);
		return new AndOrResult<TEnum, IThat<TEnum>>(subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint<TEnum>(it, grammars, state.ExpectedValues, state.ExpectedExpression)
						.InvertIf(state.Negated)),
			subject);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static AndOrResult<TEnum, IThat<TEnum>> IsOneOfForValuesCore<TEnum>(
		IThat<TEnum> subject,
		IEnumerable<TEnum> expected,
		string? expectedExpression,
		bool negated)
		where TEnum : struct, Enum
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<TEnum?>(), expectedExpression, negated);
	}

	private sealed class IsOneOfConstraint<TEnum>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TEnum?> expected,
		string? expectedExpression)
		: ConstraintResult.WithNotNullValue<TEnum>(it, grammars),
			IValueConstraint<TEnum>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => actual.Equals(value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
