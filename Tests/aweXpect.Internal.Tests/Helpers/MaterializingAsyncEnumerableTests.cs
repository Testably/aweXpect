#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class MaterializingAsyncEnumerableTests
{
	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task WhenIterating_ShouldReturnAllValues()
	{
		IAsyncEnumerable<int> enumerable = ToAsyncEnumerable([1, 2, 3]);

		IAsyncEnumerable<int> materialized = MaterializingAsyncEnumerable<int>.Wrap(enumerable, CancellationToken.None);

		await That(materialized).IsEqualTo([1, 2, 3]);
	}

	[Fact]
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

	[Fact]
	public async Task Wrap_Twice_ShouldUseSameInstance()
	{
		IAsyncEnumerable<int> enumerable = ToAsyncEnumerable([1, 2, 3]);

		IAsyncEnumerable<int> materialized1 = MaterializingAsyncEnumerable<int>.Wrap(enumerable, CancellationToken.None);
		IAsyncEnumerable<int> materialized2 = MaterializingAsyncEnumerable<int>.Wrap(materialized1, CancellationToken.None);

		await That(enumerable).IsNotSameAs(materialized1);
		await That(materialized1).IsSameAs(materialized2);
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
