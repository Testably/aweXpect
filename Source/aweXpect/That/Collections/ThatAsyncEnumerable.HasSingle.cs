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
	public static AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new HasSingleConstraint<TItem>(it, grammars, options)),
			options,
			async f =>
			{
#pragma warning disable S3267 // net8.0 has no LINQ over IAsyncEnumerable
				await foreach (TItem item in f)
				{
					if (options.Matches(item))
					{
						return item;
					}
				}
#pragma warning restore S3267

				return default;
			});
	}

	private sealed class HasSingleConstraint<TItem>(
		string it,
		ExpectationGrammars grammars,
		PredicateOptions<TItem> options)
		: ConstraintResult.WithValue<TItem?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isEmpty;
		private IMaterializedAsyncEnumerable<TItem>? _materialized;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
			_materialized = materialized as IMaterializedAsyncEnumerable<TItem>;
			_count = 0;
			_isEmpty = true;

			await foreach (TItem item in materialized.UntilCancelled(cancellationToken))
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

			if (_count <= 1 && cancellationToken.IsCanceledBeforeTheEndOf(materialized))
			{
				Outcome = Outcome.Undecided;
				_collectionContext.Set(_materialized, true);
				return this;
			}

			Outcome = _count == 1 ? Outcome.Success : Outcome.Failure;
			// The single item also explains the failure of a negation, but not of a continuation on the item.
			if (_count > 1)
			{
				_collectionContext.Set(_materialized);
			}
			else if (_count == 1)
			{
				_collectionContext.Set(_materialized?.MaterializedItems);
			}

			return this;
		}

		/// <remarks>
		///     The collection is served from the materialized items, so that the single item for further expectations
		///     does not enumerate the source again. Only the exact collection type is served, as the collection itself can
		///     also be an item (e.g. an <see langword="object" />).
		/// </remarks>
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (typeof(TValue) == typeof(IAsyncEnumerable<TItem>) &&
			    _materialized is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			return base.TryGetStoredValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a single item", "have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(It, Grammars);
			}
			else if (_count == 0)
			{
				stringBuilder.Append(It).Append(_isEmpty
					? Grammars.SubjectVerb(It, " was empty", " were empty")
					: " had no matching item");
			}
			else
			{
				stringBuilder.Append(It).Append(options.GetDescription().Length == 0
					? " had more than one item"
					: " had more than one matching item");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a single item", "do not have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(It, Grammars);
			}
			else
			{
				stringBuilder.Append(It).Append(options.GetDescription().Length == 0
					? " had the single item "
					: " had the single matching item ");
				Formatter.Format(stringBuilder, Actual);
			}
		}

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome
		{
			get => _actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}
	}
}
#endif
