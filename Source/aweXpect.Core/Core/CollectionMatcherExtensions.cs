using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Core;

/// <summary>
///     Extension methods on <see cref="ICollectionMatcher{TItem, TMatch}" />.
/// </summary>
public static class CollectionMatcherExtensions
{
	/// <summary>
	///     Verifies the <paramref name="items" /> of the subject with the <paramref name="matcher" />, comparing them
	///     with the <paramref name="options" />, and returns the outcome together with the failure text.
	/// </summary>
	/// <remarks>
	///     The items are verified one by one until the <paramref name="matcher" /> fails or
	///     <see cref="ICollectionMatcher{TItem, TMatch}.IsDetermined" />, and then completed with
	///     <see cref="ICollectionMatcher{TItem, TMatch}.VerifyComplete" />. The failure text starts with
	///     <paramref name="it" /> and is the one of the collection <c>IsEqualTo</c>, also when the matcher reports too many
	///     deviations; it is <see langword="null" /> unless the outcome is <see cref="Outcome.Failure" />.<br />
	///     The <paramref name="cancellationToken" /> leaves the outcome <see cref="Outcome.Undecided" /> when it is
	///     canceled before all <paramref name="items" /> are available, or while an expectation of a matcher from
	///     <see cref="CollectionMatchOptions.ExpectationItem{TItem}" /> items was undecided. A collection that knows its
	///     number of items, or a sequence from
	///     <see cref="EvaluationContextExtensions.UseMaterializedEnumerable{TItem}(IEvaluationContext, IEnumerable{TItem})" />
	///     that was read to its end, is verified completely like any other value.<br />
	///     The enumeration of the <paramref name="items" /> is treated as <see cref="UserCode" />. While the
	///     <paramref name="matcher" /> answers synchronously, the result completes synchronously as well.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	///     The <paramref name="matcher" />, the <paramref name="items" /> or the <paramref name="options" /> are
	///     <see langword="null" />.
	/// </exception>
	public static ValueTask<(Outcome Outcome, string? Failure)> VerifyItems<TItem, TMatch>(
		this ICollectionMatcher<TItem, TMatch> matcher, string it, IEnumerable<TItem> items,
		IEqualityEvaluator<TMatch> options, CancellationToken cancellationToken)
		where TItem : TMatch
	{
		matcher.ThrowIfNull();
		items.ThrowIfNull();
		options.ThrowIfNull();
		int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		ItemEnumerator<TItem> enumerator = new(items);
		bool isHandedOver = false;
		try
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.IsCanceledBeforeTheEnd(cancellationToken))
				{
					return new ValueTask<(Outcome, string?)>((Outcome.Undecided, null));
				}

				ValueTask<(bool, string?)> verified = matcher.Verify(it, enumerator.Current, options, maximumNumber);
				if (!verified.IsCompletedSuccessfully)
				{
					// The continuation enumerates the remaining items, so it disposes the enumerator.
					isHandedOver = true;
					return VerifyItemsAsync(verified, enumerator, matcher, it, options, maximumNumber,
						cancellationToken);
				}

				if (GetFailure(verified.Result, matcher, it, maximumNumber, cancellationToken) is { } failure)
				{
					return new ValueTask<(Outcome, string?)>((Outcome.Failure, failure));
				}

				if (matcher.IsDetermined)
				{
					break;
				}
			}
		}
		finally
		{
			if (!isHandedOver)
			{
				enumerator.Dispose();
			}
		}

		return Complete(matcher, it, options, maximumNumber, cancellationToken);
	}

	/// <summary>
	///     Verifies the remaining items of the <paramref name="enumerator" />, once the current one is
	///     <paramref name="verified" />, and disposes it.
	/// </summary>
	private static async ValueTask<(Outcome, string?)> VerifyItemsAsync<TItem, TMatch>(
		ValueTask<(bool, string?)> verified, ItemEnumerator<TItem> enumerator,
		ICollectionMatcher<TItem, TMatch> matcher, string it, IEqualityEvaluator<TMatch> options, int maximumNumber,
		CancellationToken cancellationToken)
		where TItem : TMatch
	{
		try
		{
			while (true)
			{
				if (GetFailure(await verified, matcher, it, maximumNumber, cancellationToken) is { } failure)
				{
					return (Outcome.Failure, failure);
				}

				if (matcher.IsDetermined || !enumerator.MoveNext())
				{
					break;
				}

				if (enumerator.IsCanceledBeforeTheEnd(cancellationToken))
				{
					return (Outcome.Undecided, null);
				}

				verified = matcher.Verify(it, enumerator.Current, options, maximumNumber);
			}
		}
		finally
		{
			enumerator.Dispose();
		}

		return await Complete(matcher, it, options, maximumNumber, cancellationToken);
	}

	private static ValueTask<(Outcome, string?)> Complete<TItem, TMatch>(ICollectionMatcher<TItem, TMatch> matcher,
		string it, IEqualityEvaluator<TMatch> options, int maximumNumber, CancellationToken cancellationToken)
		where TItem : TMatch
	{
		ValueTask<(bool, string?)> completed = matcher.VerifyComplete(it, options, maximumNumber);
		if (!completed.IsCompletedSuccessfully)
		{
			return CompleteAsync(completed, matcher, it, maximumNumber, cancellationToken);
		}

		return new ValueTask<(Outcome, string?)>(
			GetCompletedResult(completed.Result, matcher, it, maximumNumber, cancellationToken));
	}

	private static async ValueTask<(Outcome, string?)> CompleteAsync<TItem, TMatch>(
		ValueTask<(bool, string?)> completed, ICollectionMatcher<TItem, TMatch> matcher, string it,
		int maximumNumber, CancellationToken cancellationToken)
		where TItem : TMatch
		=> GetCompletedResult(await completed, matcher, it, maximumNumber, cancellationToken);

	/// <remarks>
	///     The final check can evaluate item expectations as well, e.g. to reassign items in any order.
	/// </remarks>
	private static (Outcome, string?) GetCompletedResult<TItem, TMatch>((bool, string?) completed,
		ICollectionMatcher<TItem, TMatch> matcher, string it, int maximumNumber, CancellationToken cancellationToken)
		where TItem : TMatch
	{
		(bool isFailure, string? failure) = completed;
		if (IsAnItemExpectationCanceled(matcher, cancellationToken))
		{
			return (Outcome.Undecided, null);
		}

		return isFailure
			? (Outcome.Failure, failure ?? TooManyDeviations(it, maximumNumber))
			: (Outcome.Success, null);
	}

	/// <summary>
	///     The failure text of a <paramref name="verified" /> item, or <see langword="null" /> when it does not fail.
	/// </summary>
	/// <remarks>
	///     A canceled item expectation does not match, which must not be reported as a mismatch.
	/// </remarks>
	private static string? GetFailure<TItem, TMatch>((bool, string?) verified,
		ICollectionMatcher<TItem, TMatch> matcher, string it, int maximumNumber, CancellationToken cancellationToken)
		where TItem : TMatch
	{
		(bool isFailure, string? failure) = verified;
		if (!isFailure || IsAnItemExpectationCanceled(matcher, cancellationToken))
		{
			return null;
		}

		return failure ?? TooManyDeviations(it, maximumNumber);
	}

	private static string TooManyDeviations(string it, int maximumNumber)
		=> $"{it} had more than {2L * maximumNumber} deviations";

	private static bool IsAnItemExpectationCanceled<TItem, TMatch>(ICollectionMatcher<TItem, TMatch> matcher,
		CancellationToken cancellationToken)
		where TItem : TMatch
		=> cancellationToken.IsCancellationRequested &&
		   matcher is CollectionMatchOptions.IExpectationItemsMatcher { HasUndecidedExpectation: true, };

	/// <summary>
	///     Enumerates the items without allocating an enumerator for an array, a <see cref="List{T}" /> or a sequence
	///     from <see cref="EvaluationContextExtensions.UseMaterializedEnumerable{TItem}(IEvaluationContext, IEnumerable{TItem})" />,
	///     as most subjects are one.
	/// </summary>
	/// <remarks>
	///     A <see cref="List{T}" /> keeps its own enumerator, so that a modification during the enumeration still throws.
	///     The enumerator of any other collection is requested, advanced and read as code of the caller, so that an
	///     exception of a subject that is not materialized fails the expectation as well.
	/// </remarks>
	private struct ItemEnumerator<TItem> : IDisposable
	{
		private readonly TItem[]? _array;
		private readonly IEnumerator<TItem>? _enumerator;
		private readonly bool _isList;
		private readonly IEnumerable<TItem> _items;
		private readonly MaterializingEnumerable<TItem>? _materialized;
		private List<TItem>.Enumerator _listEnumerator;
		private int _index;

		public ItemEnumerator(IEnumerable<TItem> items)
		{
			_items = items;
			_array = null;
			_enumerator = null;
			_isList = false;
			_materialized = null;
			_listEnumerator = default;
			_index = -1;
			Current = default!;
			if (items is TItem[] array)
			{
				_array = array;
			}
			else if (items is MaterializingEnumerable<TItem> materialized)
			{
				_materialized = materialized;
			}
			else if (items is List<TItem> list)
			{
				_isList = true;
				_listEnumerator = list.GetEnumerator();
			}
			else
			{
				_enumerator = UserCode.Invoke(static source => source.GetEnumerator(), items);
			}
		}

		public TItem Current { get; private set; }

		/// <summary>
		///     Whether the <paramref name="cancellationToken" /> is canceled before all items are available.
		/// </summary>
		/// <remarks>
		///     A cancellation only stops reading the items that still have to come from the source, so that a collection
		///     which is already complete in memory is judged like any other value.
		/// </remarks>
		public bool IsCanceledBeforeTheEnd(CancellationToken cancellationToken)
			=> cancellationToken.IsCancellationRequested &&
			   _items is not (ICollection<TItem> or IReadOnlyCollection<TItem> or ICollection) &&
			   _items is not ICountable { Count: not null, };

		public bool MoveNext()
		{
			if (_array is not null)
			{
				if (++_index >= _array.Length)
				{
					return false;
				}

				Current = _array[_index];
				return true;
			}

			if (_isList)
			{
				if (!_listEnumerator.MoveNext())
				{
					return false;
				}

				Current = _listEnumerator.Current;
				return true;
			}

			if (_materialized is not null)
			{
				if (!_materialized.TryGetItem(++_index, out TItem materializedItem))
				{
					return false;
				}

				Current = materializedItem;
				return true;
			}

			(bool hasItem, TItem item) = UserCode.Invoke(
				static enumerator => enumerator.MoveNext() ? (true, enumerator.Current) : (false, default(TItem)!),
				_enumerator!);
			Current = item;
			return hasItem;
		}

		public void Dispose()
		{
			if (_enumerator is not null)
			{
				DisposeIgnoringExceptions(_enumerator);
			}
		}

		/// <remarks>
		///     Kept apart from <see cref="Dispose()" />, which can only be inlined without exception handling.
		/// </remarks>
		private static void DisposeIgnoringExceptions(IEnumerator<TItem> enumerator)
		{
			try
			{
				enumerator.Dispose();
			}
			catch (Exception)
			{
				// The outcome is already decided, so an exception while disposing the enumerator must not replace it.
			}
		}
	}
}
