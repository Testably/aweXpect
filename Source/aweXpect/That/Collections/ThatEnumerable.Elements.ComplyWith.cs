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
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new ComplyWithConstraint<IEnumerable<TItem>?, TItem>(it, grammars, _quantifier, expectations)),
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
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new ComplyWithConstraint<IEnumerable<string?>?, string?>(it, grammars, _quantifier,
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
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new ComplyWithConstraint<TEnumerable?, object?>(it, grammars,
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
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new ComplyWithConstraint<TEnumerable, TItem>(it,
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

	/// <remarks>
	///     The items of a non-generic collection are formatted as the type of its first item that is not
	///     <see langword="null" />.
	/// </remarks>
	private sealed class ComplyWithConstraint<TEnumerable, TItem>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: ComplyWithConstraintBase<TEnumerable, TItem>(it, grammars, quantifier, expectations),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private Type? _itemType;

		/// <inheritdoc />
		protected override Type ItemType => _itemType ?? typeof(TItem);

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
			_itemType = null;
			Actual = actual;
			await PrepareExpectation(context, cancellationToken);
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				return this;
			}

			CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
			IEnumerable<TItem> items = CollectionItems<TItem>.IsTyped<TEnumerable>()
				? materialized.Items
				: WithItemType(materialized.Items);
			return await IsMetByItems(items, CollectionItems<TItem>.CountOf(actual) is null,
				() => materialized.IsCanceledBeforeTheEnd(cancellationToken),
				isIncomplete => materialized.SetContext(ref _collectionContext, isIncomplete),
				context, cancellationToken);
		}

		private IEnumerable<TItem> WithItemType(IEnumerable<TItem> items)
		{
			foreach (TItem item in items)
			{
				_itemType ??= item?.GetType();
				yield return item;
			}
		}
	}
}
