#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal sealed class MaterializingAsyncEnumerable<T> : IAsyncEnumerable<T>, IMaterializedEnumerable<T>
{
	private readonly CancellationToken _cancellationToken;
	private readonly IAsyncEnumerable<T> _enumerable;
	private readonly List<T> _materializedItems = new();
	private IAsyncEnumerator<T>? _enumerator;
	private Exception? _sourceException;

	private MaterializingAsyncEnumerable(IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
	{
		_enumerable = enumerable;
		_cancellationToken = cancellationToken;
	}

	#region IAsyncEnumerable<T> Members

	/// <remarks>
	///     All enumerations continue the same source enumerator, so the source is governed by the
	///     <see cref="CancellationToken" /> of the evaluation that wrapped it, not by the
	///     <paramref name="cancellationToken" /> of a single enumeration.<br />
	///     The source is not advanced once the evaluation is cancelled, and a pending <c>MoveNextAsync</c> is abandoned,
	///     so that a source which ignores the cancellation cannot hang the evaluation. Instead of the next item that
	///     was not received, the enumeration throws an <see cref="OperationCanceledException" />, so that the
	///     cancellation is not mistaken for the end of the source.
	/// </remarks>
	public async IAsyncEnumerator<T> GetAsyncEnumerator(
		CancellationToken cancellationToken = default)
	{
		foreach (T materializedItem in _materializedItems)
		{
			yield return materializedItem;
		}

		if (Count is not null)
		{
			yield break;
		}

		_enumerator ??= _enumerable.GetAsyncEnumerator(_cancellationToken);
		// Stryker disable once Conditional : a mutated condition keeps appending the exhausted enumerator's current item until the test host runs out of memory, which costs a minute per mutant and cannot be killed any cheaper
		while (await MoveNext(_enumerator))
		{
			T item = _enumerator.Current;
			_materializedItems.Add(item);
			cancellationToken.ThrowIfCancellationRequested();
			yield return item;
		}

		Count = _materializedItems.Count;
	}

	#endregion

	/// <inheritdoc cref="ICountable.Count" />
	public int? Count { get; private set; }

	/// <inheritdoc cref="IMaterializedEnumerable{T}.MaterializedItems" />
	IReadOnlyList<T> IMaterializedEnumerable<T>.MaterializedItems => _materializedItems;

	/// <inheritdoc cref="IMaterializedEnumerable{T}.MaterializeItems(int?)" />
	/// <remarks>
	///     A cancellation of the evaluation stops materializing and leaves the <see cref="Count" /> unknown, so that the
	///     items received so far can still be listed.
	/// </remarks>
	public async Task<IMaterializedEnumerable<T>> MaterializeItems(int? numberOfItems)
	{
		int index = 0;
		try
		{
			await foreach (T _ in this)
			{
				if (numberOfItems.HasValue && ++index > numberOfItems)
				{
					return this;
				}
			}
		}
		catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
		{
			return this;
		}

		Count = _materializedItems.Count;
		return this;
	}

	public static IAsyncEnumerable<T> Wrap(IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
	{
		if (enumerable is MaterializingAsyncEnumerable<T>)
		{
			return enumerable;
		}

		return new MaterializingAsyncEnumerable<T>(enumerable, cancellationToken);
	}

	/// <remarks>
	///     A source that threw is not advanced again, but every further enumeration throws the same exception, so that
	///     it cannot be mistaken for the end of the source.
	/// </remarks>
	private async ValueTask<bool> MoveNext(IAsyncEnumerator<T> enumerator)
	{
		if (_sourceException is not null)
		{
			ExceptionDispatchInfo.Capture(_sourceException).Throw();
		}

		try
		{
			return await UserCode.InvokeAsync(() => MoveNextOrAbandon(enumerator), _cancellationToken);
		}
		catch (Exception exception) when (!(exception is OperationCanceledException &&
		                                    _cancellationToken.IsCancellationRequested))
		{
			_sourceException = exception;
			throw;
		}
	}

	private ValueTask<bool> MoveNextOrAbandon(IAsyncEnumerator<T> enumerator)
	{
		_cancellationToken.ThrowIfCancellationRequested();
		ValueTask<bool> moveNext = enumerator.MoveNextAsync();
		return (moveNext.IsCompleted && !_cancellationToken.IsCancellationRequested) ||
		       !_cancellationToken.CanBeCanceled
			? moveNext
			: new ValueTask<bool>(AwaitOrAbandon(moveNext.AsTask()));
	}

	/// <remarks>
	///     An item that arrives once the cancellation is requested is dropped, so that it does not depend on the timing
	///     whether a cancellation during <c>MoveNextAsync</c> still yields the item.<br />
	///     An abandoned <paramref name="moveNext" /> keeps running, so its exception is observed, as nobody else awaits
	///     it and it would otherwise surface as <see cref="TaskScheduler.UnobservedTaskException" />.
	/// </remarks>
	private async Task<bool> AwaitOrAbandon(Task<bool> moveNext)
	{
		try
		{
			bool hasNext = await moveNext.WaitAsync(_cancellationToken);
			_cancellationToken.ThrowIfCancellationRequested();
			return hasNext;
		}
		catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
		{
			_ = moveNext.ContinueWith(static t => _ = t.Exception, CancellationToken.None,
				TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
			throw;
		}
	}
}

#endif
