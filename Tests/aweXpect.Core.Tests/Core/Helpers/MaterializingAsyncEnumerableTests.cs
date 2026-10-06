#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class MaterializingAsyncEnumerableTests
{
	[Test]
	public async Task ReleaseSource_ShouldOnlyReplayTheItemsReadSoFar()
	{
		MaterializingAsyncEnumerable<int> materialized = (MaterializingAsyncEnumerable<int>)
			MaterializingAsyncEnumerable<int>.Wrap(ToAsyncEnumerable([1, 2, 3]), CancellationToken.None);
		await materialized.MaterializeItems(0);

		await materialized.ReleaseSource();
		List<int> items = [];
		await foreach (int item in materialized)
		{
			items.Add(item);
		}

		await That(items).IsEqualTo([1])
			.Because("the released source must not be read any further");
		await That(materialized.Count).IsNull()
			.Because("it is unknown how many items the released source has");
	}

	[Test]
	public async Task ReleaseSource_WhenDisposingTheSourceThrows_ShouldNotThrow()
	{
		DisposeTrackingAsyncEnumerable source = new(1, 2)
		{
			DisposeException = new InvalidOperationException("dispose failed"),
		};
		MaterializingAsyncEnumerable<int> materialized = (MaterializingAsyncEnumerable<int>)
			MaterializingAsyncEnumerable<int>.Wrap(source, CancellationToken.None);
		await materialized.MaterializeItems(0);

		async Task Act() => await materialized.ReleaseSource();

		await That(Act).DoesNotThrow()
			.Because("the outcome is already decided when the source is released");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task ReleaseSource_WhenMoveNextWasAbandoned_ShouldDisposeTheSourceOnceItCompleted()
	{
		using CancellationTokenSource cts = new();
		DisposeTrackingAsyncEnumerable source = new(1)
		{
			PendingMoveNext = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously),
		};
		MaterializingAsyncEnumerable<int> materialized = (MaterializingAsyncEnumerable<int>)
			MaterializingAsyncEnumerable<int>.Wrap(source, cts.Token);
		Task materializing = materialized.MaterializeItems(null);
		await source.IsMoving.Task;
		await cts.CancelAsync();
		await materializing;

		await materialized.ReleaseSource();
		int disposeCountWhileMoving = source.DisposeCount;
		source.PendingMoveNext.SetResult(false);
		await source.Disposed.Task.WaitAsync(TimeSpan.FromSeconds(30));

		await That(disposeCountWhileMoving).IsEqualTo(0)
			.Because("the source must not be disposed while its MoveNextAsync is still running");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task ReleaseSource_WhenPartiallyRead_ShouldDisposeTheSourceOnce()
	{
		DisposeTrackingAsyncEnumerable source = new(1, 2);
		MaterializingAsyncEnumerable<int> materialized = (MaterializingAsyncEnumerable<int>)
			MaterializingAsyncEnumerable<int>.Wrap(source, CancellationToken.None);
		await materialized.MaterializeItems(0);

		await materialized.ReleaseSource();
		await materialized.ReleaseSource();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task WhenCancelledBetweenItems_ShouldNotSetTheCount()
	{
		using CancellationTokenSource cts = new();
		MaterializingAsyncEnumerable<int> materialized = (MaterializingAsyncEnumerable<int>)
			MaterializingAsyncEnumerable<int>.Wrap(ToAsyncEnumerable([1, 2, 3]), cts.Token);
		await using (IAsyncEnumerator<int> enumerator = materialized.GetAsyncEnumerator())
		{
			await enumerator.MoveNextAsync();
		}

		await cts.CancelAsync();
		await materialized.MaterializeItems(null);

		await That(materialized.Count).IsNull()
			.Because("a cancelled enumeration does not know how many items the source has");
	}

	[Test]
	public async Task WhenEnumeratedAfterCancellation_ShouldReplayTheMaterializedItemsAndThrow()
	{
		using CancellationTokenSource cts = new();
		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(ToAsyncEnumerable([1, 2, 3]), cts.Token);
		await using (IAsyncEnumerator<int> enumerator = materialized.GetAsyncEnumerator())
		{
			await enumerator.MoveNextAsync();
		}

		await cts.CancelAsync();
		List<int> items = [];

		async Task Act()
		{
			await foreach (int item in materialized)
			{
				items.Add(item);
			}
		}

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage(new OperationCanceledException().Message)
			.Because("a cancellation must not be mistaken for the end of the source");
		await That(items).IsEqualTo([1])
			.Because("the source must not be advanced once the evaluation is cancelled");
	}

	[Test]
	public async Task WhenEnumeratedAfterCancellationOfAnExhaustedSource_ShouldReplayAllItems()
	{
		using CancellationTokenSource cts = new();
		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(ToAsyncEnumerable([1, 2, 3]), cts.Token);
		await foreach (int _ in materialized)
		{
		}

		await cts.CancelAsync();
		List<int> items = [];
		await foreach (int item in materialized)
		{
			items.Add(item);
		}

		await That(items).IsEqualTo([1, 2, 3])
			.Because("the end of the source was reached before the cancellation");
	}

	[Test]
	public async Task WhenEnumeratedWhileEnumerating_ShouldYieldAllItemsToBoth()
	{
		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(ToAsyncEnumerable([1, 1, 2]), CancellationToken.None);
		List<int> outer = [];
		List<int> inner = [];

		await foreach (int item in materialized)
		{
			outer.Add(item);
			if (inner.Count == 0)
			{
				await foreach (int innerItem in materialized)
				{
					inner.Add(innerItem);
				}
			}
		}

		await That(outer).IsEqualTo([1, 1, 2])
			.Because("the outer enumeration continues after the items that the inner one read");
		await That(inner).IsEqualTo([1, 1, 2]);
	}

	[Test]
	public async Task WhenIterating_ShouldReturnAllValues()
	{
		IAsyncEnumerable<int> enumerable = ToAsyncEnumerable([1, 2, 3]);

		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(enumerable, CancellationToken.None);

		await That(materialized).IsEqualTo([1, 2, 3]);
	}

	[Test]
	public async Task WhenSourceIsEnumerated_ShouldReceiveTheTokenOfTheEvaluation()
	{
		using CancellationTokenSource cts = new();
		CancellationToken sourceToken = CancellationToken.None;

		async IAsyncEnumerable<int> Source([EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			sourceToken = cancellationToken;
			await Task.Yield();
			yield return 1;
		}

		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(Source(), cts.Token);

		await foreach (int _ in materialized)
		{
		}

		await That(sourceToken).IsEqualTo(cts.Token)
			.Because("all enumerations share the source enumerator, which is governed by the evaluation");
	}

	[Test]
	public async Task Wrap_Twice_ShouldUseSameInstance()
	{
		IAsyncEnumerable<int> enumerable = ToAsyncEnumerable([1, 2, 3]);

		IAsyncEnumerable<int> materialized1 = MaterializingAsyncEnumerable<int>.Wrap(enumerable, CancellationToken.None);
		IAsyncEnumerable<int> materialized2 = MaterializingAsyncEnumerable<int>.Wrap(materialized1, CancellationToken.None);

		await That(enumerable).IsNotSameAs(materialized1);
		await That(materialized1).IsSameAs(materialized2);
	}

	private sealed class DisposeTrackingAsyncEnumerable(params int[] values) : IAsyncEnumerable<int>
	{
		public int DisposeCount { get; private set; }

		public TaskCompletionSource<bool> Disposed { get; } =
			new(TaskCreationOptions.RunContinuationsAsynchronously);

		public Exception? DisposeException { get; set; }

		public TaskCompletionSource<bool> IsMoving { get; } =
			new(TaskCreationOptions.RunContinuationsAsynchronously);

		public TaskCompletionSource<bool>? PendingMoveNext { get; set; }

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> new Enumerator(this, values);

		private sealed class Enumerator(DisposeTrackingAsyncEnumerable owner, int[] values) : IAsyncEnumerator<int>
		{
			private int _index = -1;

			public int Current => values[_index];

			public async ValueTask<bool> MoveNextAsync()
			{
				if (++_index < values.Length)
				{
					return true;
				}

				if (owner.PendingMoveNext is null)
				{
					return false;
				}

				owner.IsMoving.TrySetResult(true);
				return await owner.PendingMoveNext.Task;
			}

			public ValueTask DisposeAsync()
			{
				owner.DisposeCount++;
				owner.Disposed.TrySetResult(true);
				if (owner.DisposeException is not null)
				{
					throw owner.DisposeException;
				}

				return default;
			}
		}
	}

	private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(T[] items)
	{
		foreach (T item in items)
		{
			await Task.Yield();
			yield return item;
		}
	}
}
#endif
