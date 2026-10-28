#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Helpers;

internal sealed class MaterializingAsyncEnumerable<T> : IMaterializedAsyncEnumerable<T>, IMaterialization
{
	private readonly CancellationToken _cancellationToken;
	private readonly IAsyncEnumerable<T> _enumerable;
	private readonly List<T> _materializedItems = new();
	private Task<bool>? _abandonedMoveNext;
	private IAsyncEnumerator<T>? _enumerator;
	private bool _isSourceReleased;
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
	///     The source is not advanced once the evaluation is canceled, and a pending <c>MoveNextAsync</c> is abandoned,
	///     so that a source which ignores the cancellation cannot hang the evaluation. Instead of the next item that
	///     was not received, the enumeration throws an <see cref="OperationCanceledException" />, so that the
	///     cancellation is not mistaken for the end of the source.<br />
	///     The items are replayed by index, so that an enumeration nested in another one continues where the other one
	///     stopped, instead of cutting it short.
	/// </remarks>
	public IAsyncEnumerator<T> GetAsyncEnumerator(
		CancellationToken cancellationToken = default)
		=> new Enumerator(this, cancellationToken);

	#endregion

	/// <inheritdoc cref="ICountable.Count" />
	public int? Count { get; private set; }

	/// <inheritdoc cref="IMaterializedAsyncEnumerable{T}.MaterializedItems" />
	IReadOnlyList<T> IMaterializedAsyncEnumerable<T>.MaterializedItems => _materializedItems;

	/// <inheritdoc cref="IMaterializedAsyncEnumerable{T}.MaterializeItems(int?)" />
	/// <remarks>
	///     A cancellation of the evaluation stops materializing and leaves the <see cref="Count" /> unknown, so that the
	///     items received so far can still be listed.
	/// </remarks>
	public async Task<IMaterializedAsyncEnumerable<T>> MaterializeItems(int? numberOfItems)
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

		return this;
	}

	/// <inheritdoc cref="IMaterialization.ReleaseSource()" />
	/// <remarks>
	///     A <c>MoveNextAsync</c> that was abandoned at the cancellation is still running, and the source must not be
	///     disposed concurrently, so it is disposed once the <c>MoveNextAsync</c> completed.
	/// </remarks>
	public Task ReleaseSource()
	{
		if (_isSourceReleased)
		{
			return Task.CompletedTask;
		}

		_isSourceReleased = true;
		if (_enumerator is null)
		{
			return Task.CompletedTask;
		}

		if (_abandonedMoveNext is { IsCompleted: false, } abandonedMoveNext)
		{
			_ = abandonedMoveNext.ContinueWith(_ => DisposeSource(_enumerator), CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			return Task.CompletedTask;
		}

		return DisposeSource(_enumerator);
	}

	private static async Task DisposeSource(IAsyncEnumerator<T> enumerator)
	{
		try
		{
			await enumerator.DisposeAsync();
		}
		catch (Exception)
		{
			// The outcome is already decided, so an exception while disposing the source must not replace it.
		}
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
	///     The enumerator of the source is requested for the first item, so that an exception of the source fails the
	///     expectation, whether it is thrown for the enumerator, while advancing it or for its current item.<br />
	///     A source that threw is not advanced again, but every further enumeration throws the same exception, so that
	///     it cannot be mistaken for the end of the source.
	/// </remarks>
	private async ValueTask<bool> MoveNext()
	{
		if (_sourceException is not null)
		{
			ExceptionDispatchInfo.Capture(_sourceException).Throw();
		}

		try
		{
			if (!await UserCode.InvokeAsync(static self => self.MoveNextOrAbandon(), this, _cancellationToken))
			{
				return false;
			}

			_materializedItems.Add(UserCode.Invoke(static enumerator => enumerator.Current, _enumerator!));
			return true;
		}
		catch (Exception exception) when (!(exception is OperationCanceledException &&
		                                    _cancellationToken.IsCancellationRequested))
		{
			_sourceException = exception;
			throw;
		}
	}

	private ValueTask<bool> MoveNextOrAbandon()
	{
		_enumerator ??= _enumerable.GetAsyncEnumerator(_cancellationToken);
		_cancellationToken.ThrowIfCancellationRequested();
		ValueTask<bool> moveNext = _enumerator.MoveNextAsync();
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
			_abandonedMoveNext = moveNext;
			_ = moveNext.ContinueWith(static t => _ = t.Exception, CancellationToken.None,
				TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
			throw;
		}
	}

	/// <summary>
	///     Enumerates the items received so far and continues the source after them.
	/// </summary>
	/// <remarks>
	///     Written by hand instead of as an async iterator, because it is smaller than the state machine of an iterator
	///     and the items are mostly received synchronously. Like an iterator, it ends once the source ended or threw.
	/// </remarks>
	private sealed class Enumerator(MaterializingAsyncEnumerable<T> source, CancellationToken cancellationToken)
		: IAsyncEnumerator<T>
	{
		private int _index = -1;
		private bool _isFinished;

		/// <inheritdoc />
		public T Current { get; private set; } = default!;

		/// <inheritdoc />
		public ValueTask<bool> MoveNextAsync()
		{
			if (_isFinished)
			{
				return new ValueTask<bool>(false);
			}

			int index = _index + 1;
			if (index < source._materializedItems.Count)
			{
				_index = index;
				Current = source._materializedItems[index];
				return new ValueTask<bool>(true);
			}

			if (source.Count is not null || source._isSourceReleased)
			{
				_isFinished = true;
				return new ValueTask<bool>(false);
			}

			ValueTask<bool> moveNext = source.MoveNext();
			if (!moveNext.IsCompletedSuccessfully)
			{
				return AwaitMoveNext(moveNext, index);
			}

			return new ValueTask<bool>(Continue(moveNext.Result, index));
		}

		/// <inheritdoc />
		public ValueTask DisposeAsync()
		{
			_isFinished = true;
			return default;
		}

		[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
		private async ValueTask<bool> AwaitMoveNext(ValueTask<bool> moveNext, int index)
		{
			bool hasNext;
			try
			{
				hasNext = await moveNext;
			}
			catch
			{
				_isFinished = true;
				throw;
			}

			return Continue(hasNext, index);
		}

		/// <summary>
		///     Continues with the item at the <paramref name="index" />, which the source just received, or ends the
		///     enumeration and the source, when it <paramref name="hasNext" /> no further item.
		/// </summary>
		private bool Continue(bool hasNext, int index)
		{
			if (!hasNext)
			{
				_isFinished = true;
				source.Count = source._materializedItems.Count;
				return false;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				_isFinished = true;
				cancellationToken.ThrowIfCancellationRequested();
			}

			_index = index;
			Current = source._materializedItems[index];
			return true;
		}
	}
}

#endif
