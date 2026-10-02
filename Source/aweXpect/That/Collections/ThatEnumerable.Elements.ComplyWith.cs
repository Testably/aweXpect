using System;
using System.Collections;
using System.Collections.Generic;
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

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<TItem>(expectationBuilder, it, grammars, _quantifier, expectations)),
				_subject);
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithConstraint<string?>(expectationBuilder, it, grammars, _quantifier,
						expectations)),
				_subject);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable?>>
			ComplyWith(Action<IThatSubject<object?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithForEnumerableConstraint<TEnumerable>(expectationBuilder, it, grammars,
						_quantifier, expectations)),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
					=> new ComplyWithForStructEnumerableConstraint<TEnumerable, TItem>(expectationBuilder, it,
						grammars, _quantifier, expectations)),
				_subject);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<TEnumerable, IThat<TEnumerable>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
			=> new ElementsForStructEnumerable<TEnumerable, string?>(_subject, _quantifier).ComplyWith(expectations);
	}

	private sealed class ComplyWithConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: ComplyWithConstraint<IEnumerable<TItem>?, TItem>(expectationBuilder, it, grammars, quantifier,
				expectations),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		public async Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			await PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			return await IsMetByItems(materialized, actual is not ICollection<TItem>,
				() => cancellationToken.IsCanceledBeforeTheEndOf(materialized),
				isIncomplete => _collectionContext.Set(materialized, isIncomplete),
				context, cancellationToken);
		}
	}

	/// <remarks>
	///     The items of a non-generic collection are formatted as the type of its first item that is not
	///     <see langword="null" />.
	/// </remarks>
	private sealed class ComplyWithForEnumerableConstraint<TEnumerable>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThatSubject<object?>> expectations)
		: ComplyWithConstraint<TEnumerable?, object?>(expectationBuilder, it, grammars, quantifier, expectations),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private Type? _itemType;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		protected override Type ItemType => _itemType ?? typeof(object);

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			await PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			return await IsMetByItems(WithItemType(materialized), actual is not ICollection,
				() => cancellationToken.IsCanceledBeforeTheEndOf(materialized),
				isIncomplete => _collectionContext.Set(materialized, isIncomplete),
				context, cancellationToken);
		}

		private IEnumerable<object?> WithItemType(IEnumerable items)
		{
			foreach (object? item in items)
			{
				_itemType ??= item?.GetType();
				yield return item;
			}
		}
	}

	private sealed class ComplyWithForStructEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: ComplyWithConstraint<TEnumerable, TItem>(expectationBuilder, it, grammars, quantifier, expectations),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : struct, IEnumerable<TItem>
	{
		private CollectionContext _collectionContext;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
		}

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			await PrepareExpectation(context, cancellationToken);
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			return await IsMetByItems(materialized, actual is not ICollection<TItem>,
				() => cancellationToken.IsCanceledBeforeTheEndOf(materialized),
				isIncomplete => _collectionContext.Set(materialized, isIncomplete),
				context, cancellationToken);
		}
	}
}
