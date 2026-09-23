using System.Collections.Generic;
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

	/// <remarks>
	///     The expression parameter only keeps the public signature; the message formats the values themselves.
	/// </remarks>
	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static ObjectEqualityResult<object?, IThat<object?>, object?> IsOneOfCore(
		IThat<object?> subject,
		IEnumerable<object?> expected,
		string? expectedExpression,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint<object?, object?>(it, grammars, expected, options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TSubject, TExpected>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TExpected?> expected,
		ObjectEqualityOptions<TSubject> options)
		: ConstraintResult.WithValue<TSubject>(it, grammars),
			IAsyncConstraint<TSubject>
	{
		public async Task<ConstraintResult> IsMetBy(TSubject actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TExpected? value in expected)
			{
				hasValues = true;
				if (await options.AreConsideredEqual(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     <c>one of</c> already implies the equality, so only a match type that compares differently names
		///     itself, e.g. <c>is equivalent to one of […]</c>.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is ").Append(options.GetItemExpectation(
				"one of " + Formatter.Format(expected).TrimCommonWhiteSpace()));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not ").Append(options.GetItemExpectation(
				"one of " + Formatter.Format(expected).TrimCommonWhiteSpace()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
