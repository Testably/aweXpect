#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static SingleItemResult<IAsyncEnumerable<TItem>, TItem>.Async HasSingle<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> source)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new SingleItemResult<IAsyncEnumerable<TItem>, TItem>.Async(
			expectationBuilder.AddConstraint((it, grammars) =>
				new HasSingleConstraint<TItem>(expectationBuilder, it, grammars, options)),
			options,
			async f =>
			{
				await foreach (TItem item in f)
				{
					if (options.Matches(item))
					{
						return item;
					}
				}

				return default;
			});
	}

	private sealed class HasSingleConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		PredicateOptions<TItem> options)
		: ConstraintResult.WithValue<TItem?>(grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isEmpty;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			_count = 0;
			_isEmpty = true;

			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				_isEmpty = false;
				if (!options.Matches(item))
				{
					continue;
				}

				Actual = item;
				if (++_count > 1)
				{
					break;
				}
			}

			Outcome = _count == 1 ? Outcome.Success : Outcome.Failure;
			if (_count > 1)
			{
				await expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
			}

			return this;
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			return base.TryGetValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a single item", "have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_count == 0)
			{
				stringBuilder.Append(it).Append(_isEmpty
					? Grammars.SubjectVerb(it, " was empty", " were empty")
					: " did not contain any matching item");
			}
			else
			{
				stringBuilder.Append(it).Append(" contained more than one item");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a single item", "do not have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}
}
#endif
