using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static SingleItemResult<IEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleConstraint<IEnumerable<TItem>?, TItem>(it, grammars, options)),
			options,
			f => f.FirstOrDefault(item => options.Matches(item))
		);
	}

	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static SingleItemResult<IEnumerable, object?> HasSingle(
		this IThat<IEnumerable?> subject)
	{
		PredicateOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleConstraint<IEnumerable, object?>(it, grammars,
					options)),
			options,
			f =>
			{
				return f.Cast<object?>().FirstOrDefault(item => options.Matches(item));
			});
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<ImmutableArray<TItem>, TItem> HasSingle<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new SingleItemResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleConstraint<ImmutableArray<TItem>, TItem>(it, grammars,
					options)),
			options,
			f =>
			{
				return f.FirstOrDefault(item => options.Matches(item));
			});
	}
#endif

	private sealed class HasSingleConstraint<TEnumerable, TItem>(
		string it,
		ExpectationGrammars grammars,
		PredicateOptions<TItem> options)
		: ConstraintResult.WithValue<TItem?>(it, grammars),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private bool _isNull;
		private int _count;
		private bool _isEmpty;
		private object? _materialized;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_materialized = null;
			_isNull = actual is null;
			if (actual.IsDefaultImmutableArray())
			{
				return Task.FromResult(this.AsNullSubject(It));
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
			_materialized = materialized.Value;
			_count = 0;
			_isEmpty = true;

			foreach (TItem item in materialized.Items)
			{
				if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
				{
					Outcome = Outcome.Undecided;
					materialized.SetContext(ref _collectionContext, true);
					return Task.FromResult<ConstraintResult>(this);
				}

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
			// The single item also explains the failure of a negation, but not of a continuation on the item.
			if (_count > 0)
			{
				materialized.SetContext(ref _collectionContext);
			}

			return Task.FromResult<ConstraintResult>(this);
		}

		/// <remarks>
		///     The collection is served from the materialized items, so that the single item for further expectations
		///     does not enumerate the source again. Only the exact collection type is served, as the collection itself can
		///     also be an item (e.g. an <see langword="object" />).
		/// </remarks>
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (typeof(TValue) == typeof(TEnumerable) &&
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
			if (_isNull)
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
			if (_isNull)
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
			get => _isNull ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}
	}
}
