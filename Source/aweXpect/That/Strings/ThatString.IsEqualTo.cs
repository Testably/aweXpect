using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsEqualTo(
		this IThat<string?> subject,
		string? expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsNotEqualTo(
		this IThat<string?> subject,
		string? unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithEqualToValue<string?>(it, grammars, expected is null),
			IAsyncConstraint<string?>
	{
		/// <remarks>
		///     A match type that inspects the content of the subject, e.g. a prefix or a pattern, cannot answer for a
		///     <see langword="null" /> subject, because it has no content.
		/// </remarks>
		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = await options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			if (actual is null && options.InspectsSubject)
			{
				Outcome = Outcome.FailureBothWays;
			}

			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(Actual))
			{
				contexts.AddStringContextCopy("Actual", Actual, this);
				contexts.AddStringContextCopy("Expected", expected, this);
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual, expected)
				.Indent(indentation, false));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual, expected)
				.Indent(indentation, false));
	}
}
