using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Internal.Tests.Collections;

public sealed class CollectionHelpersTests
{
	public enum Kind
	{
		List,
		Queue,
		Stack,
		ReadOnlyCollection,
		ConcurrentQueue,
	}

	private static readonly CancellationToken Canceled = new(true);

	[Test]
	[Arguments(Kind.List)]
	[Arguments(Kind.Queue)]
	[Arguments(Kind.Stack)]
	[Arguments(Kind.ReadOnlyCollection)]
	[Arguments(Kind.ConcurrentQueue)]
	public async Task CountUnlessCanceled_WhenCanceled_ShouldCountACollectionThatKnowsItsCount(Kind kind)
	{
		IEnumerable<int> subject = Create(kind, 1, 2, 3);

		int? result = subject.CountUnlessCanceled(Canceled);

		await That(result).IsEqualTo(3);
	}

	[Test]
	public async Task CountUnlessCanceled_WhenCanceled_ShouldNotCountALazySequence()
	{
		IEnumerable<int> subject = Lazy(1, 2, 3);

		int? result = subject.CountUnlessCanceled(Canceled);

		await That(result).IsNull();
	}

	[Test]
	public async Task CountUnlessCanceled_WhenNotCanceled_ShouldCountALazySequence()
	{
		IEnumerable<int> subject = Lazy(1, 2, 3);

		int? result = subject.CountUnlessCanceled(CancellationToken.None);

		await That(result).IsEqualTo(3);
	}

	[Test]
	[Arguments(Kind.List)]
	[Arguments(Kind.Queue)]
	[Arguments(Kind.Stack)]
	[Arguments(Kind.ReadOnlyCollection)]
	[Arguments(Kind.ConcurrentQueue)]
	public async Task IsCanceledBeforeTheEndOf_WhenCanceled_ShouldBeFalseForACollectionThatKnowsItsCount(Kind kind)
	{
		IEnumerable<int> subject = Create(kind, 1, 2, 3);

		bool result = Canceled.IsCanceledBeforeTheEndOf(subject);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsCanceledBeforeTheEndOf_WhenCanceled_ShouldBeTrueForALazySequence()
	{
		IEnumerable<int> subject = Lazy(1, 2, 3);

		bool result = Canceled.IsCanceledBeforeTheEndOf(subject);

		await That(result).IsTrue();
	}

	[Test]
	public async Task IsCanceledBeforeTheEndOf_WhenCanceled_ShouldBeTrueUntilAMaterializingSequenceIsReadToItsEnd()
	{
		IEnumerable<int> subject = MaterializingEnumerable<int>.WrapParameter(Lazy(1, 2, 3));

		bool resultBeforeEnumeration = Canceled.IsCanceledBeforeTheEndOf(subject);
		_ = subject.ToList();
		bool resultAfterEnumeration = Canceled.IsCanceledBeforeTheEndOf(subject);

		await That(resultBeforeEnumeration).IsTrue();
		await That(resultAfterEnumeration).IsFalse();
	}

	[Test]
	[Arguments(Kind.List)]
	[Arguments(Kind.Queue)]
	[Arguments(Kind.Stack)]
	[Arguments(Kind.ReadOnlyCollection)]
	[Arguments(Kind.ConcurrentQueue)]
	public async Task IsCanceledBeforeTheEndOf_WhenNotCanceled_ShouldBeFalse(Kind kind)
	{
		IEnumerable<int> subject = Create(kind, 1, 2, 3);

		bool result = CancellationToken.None.IsCanceledBeforeTheEndOf(subject);
		bool resultForLazy = CancellationToken.None.IsCanceledBeforeTheEndOf(Lazy(1, 2, 3));

		await That(result).IsFalse();
		await That(resultForLazy).IsFalse();
	}

	[Test]
	[Arguments(Kind.List)]
	[Arguments(Kind.Queue)]
	[Arguments(Kind.Stack)]
	[Arguments(Kind.ReadOnlyCollection)]
	[Arguments(Kind.ConcurrentQueue)]
	public async Task IsCanceledBeforeTheEndOf_WhenUntypedAndCanceled_ShouldBeFalseForACollectionThatKnowsItsCount(
		Kind kind)
	{
		IEnumerable subject = Create(kind, 1, 2, 3);

		bool result = Canceled.IsCanceledBeforeTheEndOf(subject);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsCanceledBeforeTheEndOf_WhenUntypedAndCanceled_ShouldBeTrueForALazySequence()
	{
		IEnumerable subject = Lazy(1, 2, 3);

		bool result = Canceled.IsCanceledBeforeTheEndOf(subject);

		await That(result).IsTrue();
	}

	[Test]
	[Arguments(Kind.List)]
	[Arguments(Kind.Queue)]
	[Arguments(Kind.Stack)]
	[Arguments(Kind.ReadOnlyCollection)]
	[Arguments(Kind.ConcurrentQueue)]
	public async Task TryCountForIndex_WhenCanceled_ShouldCountACollectionThatKnowsItsCount(Kind kind)
	{
		IEnumerable<int> subject = Create(kind, 1, 2, 3);

		bool result = ThatEnumerable.TryCountForIndex(FromEnd(), subject, subject, Canceled, out int? count);

		await That(result).IsTrue();
		await That(count).IsEqualTo(3);
	}

	[Test]
	public async Task TryCountForIndex_WhenCanceled_ShouldNotCountALazySequence()
	{
		IEnumerable<int> subject = Lazy(1, 2, 3);

		bool result = ThatEnumerable.TryCountForIndex(FromEnd(), subject, subject, Canceled, out int? count);

		await That(result).IsFalse();
		await That(count).IsNull();
	}

	[Test]
	public async Task TryCountForIndex_WhenTheIndexIsNotCountedFromTheEnd_ShouldNotCount()
	{
		CollectionIndexOptions options = new();
		options.AtIndex(0);
		IEnumerable<int> subject = Create(Kind.Queue, 1, 2, 3);

		bool result = ThatEnumerable.TryCountForIndex(options, subject, subject, Canceled, out int? count);

		await That(result).IsTrue();
		await That(count).IsNull();
	}

	[Test]
	public async Task TryCountForIndex_WhenTheSubjectAlsoCountsItemsOfAnotherType_ShouldCountTheItemsThatAreRead()
	{
		IEnumerable<int> subject = new CollectionOfTwoItemTypes([1, 2, 3,], ["a",]);

		bool result = ThatEnumerable.TryCountForIndex(FromEnd(), subject, subject, CancellationToken.None,
			out int? count);

		await That(result).IsTrue();
		await That(count).IsEqualTo(3);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task UntilCancelled_ShouldPassTheCancellationTokenToTheSource()
	{
		using CancellationTokenSource cts = new();
		TestAsyncSource source = new(1, 2);

		List<int> items = await ReadAll(source.UntilCancelled(cts.Token));

		await That(items).IsEqualTo([1, 2,]);
		await That(source.Tokens).IsEqualTo([cts.Token,])
			.Because("the source is canceled together with the evaluation");
	}

	[Test]
	public async Task UntilCancelled_WhenCanceledWhileWaitingForTheNextItem_ShouldEndTheEnumeration()
	{
		using CancellationTokenSource cts = new();
		TaskCompletionSource<bool> next = new();
		TestAsyncSource source = new(1)
		{
			NextAfterItems = () => new ValueTask<bool>(next.Task),
		};
		IAsyncEnumerator<int> enumerator = source.UntilCancelled(cts.Token).GetAsyncEnumerator();

		bool first = await enumerator.MoveNextAsync();
		ValueTask<bool> pending = enumerator.MoveNextAsync();
		cts.Cancel();
		next.SetException(new OperationCanceledException(cts.Token));
		bool second = await pending;
		await enumerator.DisposeAsync();

		await That(first).IsTrue();
		await That(second).IsFalse()
			.Because("a source that throws because of the cancellation ends the enumeration");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task UntilCancelled_WhenEnumeratedTwice_ShouldReadTheSourceAgain()
	{
		TestAsyncSource source = new(1, 2);
		IAsyncEnumerable<int> enumerable = source.UntilCancelled(CancellationToken.None);

		List<int> first = await ReadAll(enumerable);
		List<int> second = await ReadAll(enumerable);

		await That(first).IsEqualTo([1, 2,]);
		await That(second).IsEqualTo([1, 2,])
			.Because("every enumeration gets an enumerator of its own");
		await That(source.DisposeCount).IsEqualTo(2);
	}

	[Test]
	public async Task UntilCancelled_WhenNotEnumerated_ShouldNotReadTheSource()
	{
		TestAsyncSource source = new(1, 2);
		IAsyncEnumerator<int> enumerator =
			source.UntilCancelled(CancellationToken.None).GetAsyncEnumerator();

		await enumerator.DisposeAsync();

		await That(source.Tokens).IsEmpty();
		await That(source.DisposeCount).IsEqualTo(0);
	}

	[Test]
	public async Task UntilCancelled_WhenStoppedEarly_ShouldDisposeTheSource()
	{
		TestAsyncSource source = new(1, 2, 3);

		await foreach (int _ in source.UntilCancelled(CancellationToken.None))
		{
			break;
		}

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task
		UntilCancelled_WhenTheSourceThrowsAnOperationCanceledExceptionWithoutACancellation_ShouldRethrowIt()
	{
		TestAsyncSource source = new(1)
		{
			NextAfterItems = () => ThrowAsync(new OperationCanceledException("not canceled by the evaluation")),
		};

		async Task Act() => await ReadAll(source.UntilCancelled(CancellationToken.None));

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("not canceled by the evaluation");
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the source is also disposed when it throws");
	}

	[Test]
	public async Task
		UntilCancelled_WhenTheSourceThrowsSynchronouslyBecauseOfTheCancellation_ShouldEndTheEnumeration()
	{
		using CancellationTokenSource cts = new();
		TestAsyncSource source = new(1)
		{
			NextAfterItems = () =>
			{
				cts.Cancel();
				throw new OperationCanceledException(cts.Token);
			},
		};

		List<int> items = await ReadAll(source.UntilCancelled(cts.Token));

		await That(items).IsEqualTo([1,]);
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task UntilCancelled_WhenTheSourceThrowsSynchronously_ShouldRethrowAndDisposeTheSource()
	{
		TestAsyncSource source = new(1)
		{
			NextAfterItems = () => throw new InvalidOperationException("the source failed"),
		};

		async Task Act() => await ReadAll(source.UntilCancelled(CancellationToken.None));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("the source failed");
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the source is also disposed when it throws");
	}
#endif

	private static CollectionIndexOptions FromEnd()
	{
		CollectionIndexOptions options = new();
		options.AtIndexFromEnd(0);
		return options;
	}

	private static IEnumerable<int> Create(Kind kind, params int[] items)
		=> kind switch
		{
			Kind.Queue => new Queue<int>(items),
			Kind.Stack => new Stack<int>(Enumerable.Reverse(items)),
			Kind.ReadOnlyCollection => new ReadOnlyItems(items),
			Kind.ConcurrentQueue => new ConcurrentQueue<int>(items),
			_ => new List<int>(items),
		};

	private static IEnumerable<int> Lazy(params int[] items)
	{
		foreach (int item in items)
		{
			yield return item;
		}
	}

#if NET8_0_OR_GREATER
	private static async Task<List<int>> ReadAll(IAsyncEnumerable<int> source)
	{
		List<int> items = [];
		await foreach (int item in source)
		{
			items.Add(item);
		}

		return items;
	}

	private static async ValueTask<bool> ThrowAsync(Exception exception)
	{
		await Task.Yield();
		throw exception;
	}
#endif

	/// <summary>
	///     A collection that only implements <see cref="IReadOnlyCollection{T}" />.
	/// </summary>
	private sealed class ReadOnlyItems(params int[] items) : IReadOnlyCollection<int>
	{
		public int Count => items.Length;

		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <summary>
	///     A collection of <see cref="int" /> items that also is a collection of another number of
	///     <see cref="string" /> items.
	/// </summary>
	private sealed class CollectionOfTwoItemTypes(int[] items, string[] otherItems)
		: IReadOnlyCollection<int>, IReadOnlyCollection<string>
	{
		int IReadOnlyCollection<int>.Count => items.Length;

		int IReadOnlyCollection<string>.Count => otherItems.Length;

		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();

		IEnumerator<string> IEnumerable<string>.GetEnumerator() => ((IEnumerable<string>)otherItems).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
#if NET8_0_OR_GREATER

	/// <summary>
	///     An asynchronous source of <see cref="int" /> items that records how it is enumerated and disposed.
	/// </summary>
	private sealed class TestAsyncSource(params int[] items) : IAsyncEnumerable<int>
	{
		public int DisposeCount { get; private set; }
		public Func<ValueTask<bool>>? NextAfterItems { get; init; }
		public List<CancellationToken> Tokens { get; } = [];

		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			Tokens.Add(cancellationToken);
			return new Enumerator(this, items);
		}

		private sealed class Enumerator(TestAsyncSource source, int[] items) : IAsyncEnumerator<int>
		{
			private int _index = -1;

			public int Current => items[_index];

			public ValueTask DisposeAsync()
			{
				source.DisposeCount++;
				return default;
			}

			public ValueTask<bool> MoveNextAsync()
			{
				if (++_index < items.Length)
				{
					return new ValueTask<bool>(true);
				}

				return source.NextAfterItems?.Invoke() ?? new ValueTask<bool>(false);
			}
		}
	}
#endif
}
