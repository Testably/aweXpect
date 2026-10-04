using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatObject
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static ObjectEqualityResult<object?, IThat<object?>, object?> IsOneOfCore(
		IThat<object?> subject,
		IEnumerable<object?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<object?> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars)
					=> new IsOneOfConstraint<object?, object?>(it, grammars, state.ExpectedValues,
							state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static ObjectEqualityResult<T, IThat<T>, T> IsOneOfForReferenceCore<T>(
		IThat<T> subject,
		IEnumerable<T?> expected,
		string? expectedExpression,
		bool negated)
		where T : class?
	{
		IEnumerable<T?> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<T> options = new();
		return new ObjectEqualityResult<T, IThat<T>, T>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Options: options,
					Negated: negated),
				static (state, it, grammars)
					=> new IsOneOfConstraint<T, T>(it, grammars, state.ExpectedValues,
							state.ExpectedExpression, state.Options)
						.InvertIf(state.Negated)),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TSubject, TExpected>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TExpected?> expected,
		string? expectedExpression,
		ObjectEqualityOptions<TSubject> options)
		: ConstraintResult.WithValue<TSubject>(it, grammars),
			IAsyncConstraint<TSubject>
	{
		private IObjectMatchResult? _matchResult;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());
			options.AppendContexts(contexts);
		}

		/// <remarks>
		///     Every candidate is compared with an explanation, as the failure message explains the comparison with a
		///     single candidate, or with the matching one when negated.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			foreach (TExpected? value in expected)
			{
				_matchResult = await options.AreConsideredEqualWithExplanation(actual, value);
				if (_matchResult.IsMatch)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			Outcome = Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     <c>one of</c> already implies the equality, so only a match type that compares differently names
		///     itself, e.g. <c>is equivalent to one of […]</c>.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is ", "are ")).Append(options.GetItemExpectation(
				"one of " + (expectedExpression ?? Formatter.Format(expected)).TrimCommonWhiteSpace()));

		/// <remarks>
		///     The match type explains how the subject differs from a single candidate, but the differences to the last
		///     of several candidates would read as if it were the only one, so then the subject is described instead.
		/// </remarks>
		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			TExpected?[] candidates = expected.Take(2).ToArray();
			stringBuilder.Append(candidates.Length == 1
				? _matchResult!.GetExtendedFailure(It, Grammars, Actual, candidates[0])
				: $"{It}{Grammars.SubjectVerb(It, " was ", " were ")}{Formatter.Format(Actual, FormattingOptions.Indented())}");
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not ", "are not ")).Append(options.GetItemExpectation(
				"one of " + (expectedExpression ?? Formatter.Format(expected)).TrimCommonWhiteSpace()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));
	}
}
