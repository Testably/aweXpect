using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static AndOrResult<TEnum?, IThat<TEnum?>> IsOneOfCore<TEnum>(
		IThat<TEnum?> subject,
		IEnumerable<TEnum?> expected,
		bool negated)
		where TEnum : struct, Enum
	{
		IEnumerable<TEnum?> expectedValues = expected.ToNonEmptyValues(negated);
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, expectedValues).InvertIf(negated)),
			subject);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static AndOrResult<TEnum?, IThat<TEnum?>> IsOneOfForValuesCore<TEnum>(
		IThat<TEnum?> subject,
		IEnumerable<TEnum> expected,
		bool negated)
		where TEnum : struct, Enum
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<TEnum?>(), negated);
	}

	private sealed class IsOneOfConstraint<TEnum>(string it, ExpectationGrammars grammars, IEnumerable<TEnum?> expected)
		: ConstraintResult.WithValue<TEnum?>(it, grammars),
			IValueConstraint<TEnum?>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum? actual)
		{
			Actual = actual;
			Outcome = expected.Any(value => actual.Equals(value))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
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
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
