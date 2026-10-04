using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

namespace aweXpect;

/// <summary>
///     The texts of <c>IsEmpty</c>, shared by the synchronous and the asynchronous collections.
/// </summary>
internal abstract class IsEmptyConstraintBase<TValue>(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<TValue>(it, grammars)
{
	/// <summary>
	///     Appends the items that were found.
	/// </summary>
	protected abstract void AppendItems(StringBuilder stringBuilder);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("is empty", "are empty"));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
		AppendItems(stringBuilder);
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("is not empty", "are not empty"));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was empty", " were empty"));
}

internal sealed class IsEmptyConstraint<TEnumerable, TItem>(string it, ExpectationGrammars grammars)
	: IsEmptyConstraintBase<TEnumerable>(it, grammars),
		IContextConstraint<TEnumerable>
	where TEnumerable : IEnumerable?
{
	private object? _items;

	public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
	{
		Actual = actual;
		_items = null;
		if (actual.IsDefaultImmutableArray())
		{
			return this.AsNullSubject(It);
		}

		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		if (CollectionItems<TItem>.CountOf(actual) is { } count)
		{
			_items = actual;
			Outcome = count > 0 ? Outcome.Failure : Outcome.Success;
			return this;
		}

		CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
		_items = materialized.Value;
		using IEnumerator<TItem> enumerator = materialized.Items.GetEnumerator();
		Outcome = enumerator.MoveNext() ? Outcome.Failure : Outcome.Success;
		return this;
	}

	protected override void AppendItems(StringBuilder stringBuilder)
		=> Formatter.Format(stringBuilder, _items, FormattingOptions.MultipleLines);
}

#if NET8_0_OR_GREATER
internal sealed class AsyncIsEmptyConstraint<TItem>(string it, ExpectationGrammars grammars)
	: IsEmptyConstraintBase<IAsyncEnumerable<TItem>?>(it, grammars),
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
{
	private IMaterializedAsyncEnumerable<TItem>? _materialized;

	public async ValueTask<ConstraintResult> IsMetBy(
		IAsyncEnumerable<TItem>? actual,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		_materialized = null;
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
		}
		else
		{
			Outcome = cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable)
				? Outcome.Undecided
				: Outcome.Success;
		}

		return this;
	}

	protected override void AppendItems(StringBuilder stringBuilder)
		=> stringBuilder.Append(_materialized?.FormatMaterializedItems(FormattingOptions.MultipleLines));
}
#endif
