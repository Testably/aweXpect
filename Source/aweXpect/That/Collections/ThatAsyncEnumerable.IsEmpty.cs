#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsEmpty<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEmptyConstraint<TItem>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotEmpty<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEmptyConstraint<TItem>(it, grammars).Invert()),
			subject);

	private sealed class IsEmptyConstraint<TItem>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private IMaterializedAsyncEnumerable<TItem>? _materialized;

		public async Task<ConstraintResult> IsMetBy(
			IAsyncEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			_materialized = materializedEnumerable as IMaterializedAsyncEnumerable<TItem>;
			await using IAsyncEnumerator<TItem> enumerator =
				materializedEnumerable.UntilCancelled(cancellationToken).GetAsyncEnumerator(CancellationToken.None);
			if (await enumerator.MoveNextAsync())
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is empty", "are empty"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			stringBuilder.Append(_materialized?.FormatMaterializedItems(FormattingOptions.MultipleLines));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not empty", "are not empty"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was empty", " were empty"));
	}
}
#endif
