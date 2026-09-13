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
	///     Verifies that the subject starts with the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	public static StringEqualityResult<string?, IThat<string?>> StartsWith(
		this IThat<string?> source,
		string expected)
	{
		StringEqualityOptions options = new StringEqualityOptions().AsPrefix();
		return new StringEqualityResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new StartsWithConstraint(expectationBuilder, it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not start with the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	public static StringEqualityResult<string?, IThat<string?>> DoesNotStartWith(
		this IThat<string?> source,
		string unexpected)
	{
		StringEqualityOptions options = new StringEqualityOptions().AsPrefix();
		return new StringEqualityResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new StartsWithConstraint(expectationBuilder, it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	private sealed class StartsWithConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = await options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			if (!string.IsNullOrEmpty(actual))
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", actual));

				if (Outcome != Outcome.Success && !string.IsNullOrEmpty(expected))
				{
					expectationBuilder.AddContext(new ResultContext.Fixed("Expected", expected));
				}
			}

			return this;
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
