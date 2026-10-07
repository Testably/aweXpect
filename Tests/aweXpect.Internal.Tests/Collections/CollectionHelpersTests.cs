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
}
