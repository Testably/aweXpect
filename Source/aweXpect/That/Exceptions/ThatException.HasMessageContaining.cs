using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception has a message containing the <paramref name="expected" /> pattern.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Exception?, IThat<Exception?>> HasMessageContaining(
		this IThat<Exception?> source,
		string? expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Exception?, IThat<Exception?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasMessageContainingConstraint(
					expectationBuilder, it, grammars, expected, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the actual exception does not have a message containing the <paramref name="unexpected" /> pattern.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Exception?, IThat<Exception?>> DoesNotHaveMessageContaining(
		this IThat<Exception?> source,
		string? unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Exception?, IThat<Exception?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new HasMessageContainingConstraint(
					expectationBuilder, it, grammars, unexpected, options).Invert()),
			source,
			options);
	}

	internal class HasMessageContainingConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<Exception?>(it, grammars),
			IAsyncConstraint<Exception?>
	{
		public async Task<ConstraintResult> IsMetBy(Exception? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			options.AsWildcard();
			Outcome = expected is null || await options.AreConsideredEqual(actual?.Message, $"*{expected}*")
				? Outcome.Success
				: Outcome.Failure;
			if (!string.IsNullOrEmpty(actual?.Message))
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Message", actual.Message));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			ExpectationGrammars equalityGrammars = Grammars;
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with Message containing matching ");
				equalityGrammars &= ~ExpectationGrammars.Active;
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("Message contains matching ");
			}
			else
			{
				stringBuilder.Append("contains Message matching ");
			}

			options.Exactly();
			stringBuilder.Append(options.GetExpectation(expected, equalityGrammars)["equal to ".Length..]);
			options.AsWildcard();
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual?.Message, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			ExpectationGrammars equalityGrammars = Grammars;
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with Message not containing matching ");
				equalityGrammars &= ~ExpectationGrammars.Active;
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("Message does not contain matching ");
			}
			else
			{
				stringBuilder.Append("does not contain Message matching ");
			}

			options.Exactly();
			stringBuilder.Append(options.GetExpectation(expected, equalityGrammars.Negate())["equal to ".Length..]);
			options.AsWildcard();
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
