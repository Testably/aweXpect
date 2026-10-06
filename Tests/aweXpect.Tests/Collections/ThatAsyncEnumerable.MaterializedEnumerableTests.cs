#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class MaterializedEnumerableTests
	{
		[Test]
		public async Task IsEqualTo_ShouldDisposeTheEnumerator()
		{
			DisposeCountingAsyncEnumerable subject = new(1, 2, 3);

			await That(subject).IsEqualTo([1, 2, 3,]);

			await That(subject.DisposeCount).IsEqualTo(1);
		}

		[Test]
		public async Task IsNotEmpty_WhenStoppingEarly_ShouldDisposeTheEnumerator()
		{
			DisposeCountingAsyncEnumerable subject = new(1, 2, 3);

			await That(subject).IsNotEmpty();

			await That(subject.DisposeCount).IsEqualTo(1)
				.Because("the source is released after the evaluation, although it was not read to its end");
		}
	}

	private sealed class DisposeCountingAsyncEnumerable(params int[] items) : IAsyncEnumerable<int>
	{
		public int DisposeCount { get; private set; }

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> new Enumerator(this, items);

		private sealed class Enumerator(DisposeCountingAsyncEnumerable owner, int[] items) : IAsyncEnumerator<int>
		{
			private int _index = -1;

			public int Current => items[_index];

			public ValueTask<bool> MoveNextAsync()
				=> new(++_index < items.Length);

			public ValueTask DisposeAsync()
			{
				owner.DisposeCount++;
				return default;
			}
		}
	}
}
#endif
