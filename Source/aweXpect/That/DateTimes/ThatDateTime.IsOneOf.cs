using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDateTime
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTime, IThat<DateTime>> IsOneOfCore(
		IThat<DateTime> subject,
		IEnumerable<DateTime?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<DateTime?> expectedValues = expected.ToNonEmptyValues(negated);
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint(it, grammars, state.ExpectedValues, state.ExpectedExpression)
						.InvertIf(state.Negated),
				out IsOneOfConstraint constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTime, IThat<DateTime>> IsOneOfForValuesCore(
		IThat<DateTime> subject,
		IEnumerable<DateTime> expected,
		string? expectedExpression,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateTime?>(), expectedExpression, negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateTime?> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<DateTime>(it, grammars),
			IValueConstraint<DateTime>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime actual)
		{
			Actual = actual;
			_incompatibleKind = null;
			TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
			bool hasComparableValue = false;
			DateTimeKind? incomparableKind = null;
			foreach (DateTime? value in expected)
			{
				if (value is null)
				{
					continue;
				}

				if (!actual.IsKindCompatibleWith(value.Value))
				{
					incomparableKind = value.Value.Kind;
					continue;
				}

				hasComparableValue = true;
				if (actual - value.Value <= timeTolerance &&
				    actual - value.Value >= timeTolerance.Negate())
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_incompatibleKind = hasComparableValue ? null : incomparableKind;
			Outcome = Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentTolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had kind ").Append(Actual.Kind)
					.Append(", which cannot be compared with ").Append(_incompatibleKind);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
				Formatter.Format(stringBuilder, Actual);
				stringBuilder.AppendTimeDifferenceToClosest(Actual, expected);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
