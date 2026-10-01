using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatString
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static StringEqualityTypeResult<string?, IThat<string?>> IsOneOfCore(
		IThat<string?> subject,
		IEnumerable<string?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint(it, grammars, expectedValues, expectedExpression, options)
					.WithExpectedValuesContext(subject, expectedExpression, negated, expectedValues)
					.InvertIf(negated)),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<string?> expectedValues,
		string? expectedExpression,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		private bool _hasNothingToInspect;

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		/// <remarks>
		///     A match type that inspects the content of the subject, e.g. a prefix or a pattern, fails for a
		///     <see langword="null" /> subject in both polarities, because it has no content.
		/// </remarks>
		public override Outcome Outcome
		{
			get => _hasNothingToInspect ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			StringEqualityOptions stringEqualityOptions = options;
			foreach (string? value in expectedValues)
			{
				if (await stringEqualityOptions
					    .AreConsideredEqual(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_hasNothingToInspect = actual is null && options.InspectsSubject;
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues));
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
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
