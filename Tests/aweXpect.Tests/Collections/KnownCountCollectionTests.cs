using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

/// <summary>
///     A collection that knows its number of items without being an <see cref="ICollection{T}" />, e.g. a
///     <see cref="Queue{T}" />, is evaluated like a <see cref="List{T}" />: it is read as it is, its number of items
///     is reported exactly and all its items are verified and listed.
/// </summary>
public sealed class KnownCountCollection
{
	public enum Kind
	{
		List,
		Queue,
		Stack,
		ReadOnlyCollection,
		ConcurrentQueue,
	}

	private static IEnumerable<int> Create(Kind kind, params int[] items)
		=> kind switch
		{
			Kind.Queue => new Queue<int>(items),
			Kind.Stack => new Stack<int>(Enumerable.Reverse(items)),
			Kind.ReadOnlyCollection => new CountingReadOnlyCollection<int>(items),
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
	private sealed class CountingReadOnlyCollection<T>(params T[] items) : IReadOnlyCollection<T>
	{
		public int Enumerations { get; private set; }

		public int Count => items.Length;

		public IEnumerator<T> GetEnumerator()
		{
			Enumerations++;
			return ((IEnumerable<T>)items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <summary>
	///     A collection that only implements <see cref="ICollection{T}" />.
	/// </summary>
	private sealed class CountingCollection<T>(params T[] items) : ICollection<T>
	{
		public int Enumerations { get; private set; }

		public int Count => items.Length;

		public bool IsReadOnly => true;

		public IEnumerator<T> GetEnumerator()
		{
			Enumerations++;
			return ((IEnumerable<T>)items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item) => throw new NotSupportedException();

		public void Clear() => throw new NotSupportedException();

		public bool Contains(T item) => Array.IndexOf(items, item) >= 0;

		public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

		public bool Remove(T item) => throw new NotSupportedException();
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

	public sealed class HasCountTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task Between_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().Between(1).And(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has between 1 and 3 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task EqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().EqualTo(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task Exactly_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task GreaterThan_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().GreaterThan(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has more than 5 items,
				             but it had only 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task GreaterThanOrEqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().GreaterThanOrEqualTo(6);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has at least 6 items,
				             but it had only 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task LessThan_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().LessThan(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has fewer than 5 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task LessThanOrEqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().LessThanOrEqualTo(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has at most 3 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotBetween_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotBetween(1).And(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have between 1 and 5 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotEqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotEqualTo(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have exactly 5 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotGreaterThan_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotGreaterThan(4);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have more than 4 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotGreaterThanOrEqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotGreaterThanOrEqualTo(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have at least 5 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotLessThan_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotLessThan(6);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have fewer than 6 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task NotLessThanOrEqualTo_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount().NotLessThanOrEqualTo(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have at most 5 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}
	}

	public sealed class IsEmptyTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task IsEmpty_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was [
				               1,
				               2,
				               3,
				               4,
				               5
				             ]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task IsNotEmpty_ShouldFailForAnEmptyCollection(Kind kind)
		{
			IEnumerable<int> subject = Create(kind);

			async Task Act()
				=> await That(subject).IsNotEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it was empty
				             """);
		}
	}

	public sealed class QuantifierTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_AreEqualTo_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).All().AreEqualTo(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for all items,
				             but only 1 of 5 were

				             Not matching items:
				             [2, 3, 4, 5]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_AreUnique_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 1, 3, 3);

			async Task Act()
				=> await That(subject).All().AreUnique();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is unique for all items,
				             but only 1 of 5 were

				             Not matching items:
				             [1, 1, 3, 3]

				             Collection:
				             [1, 2, 1, 3, 3]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_ComplyWith_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.IsLessThan(3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is less than 3 for all items,
				             but only 2 of 5 were

				             Not matching items:
				             [3, 4, 5]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).All().Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for all items,
				             but only 2 of 5 did

				             Not matching items:
				             [3, 4, 5]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task AtLeast_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).AtLeast(3).Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for at least 3 items,
				             but only 2 of 5 did

				             Not matching items:
				             [3, 4, 5]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task AtMost_AreEqualTo_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 1, 3, 3);

			async Task Act()
				=> await That(subject).AtMost(1).AreEqualTo(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for at most one item,
				             but 2 of 5 were

				             Matching items:
				             [1, 1]

				             Collection:
				             [1, 2, 1, 3, 3]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task AtMost_ComplyWith_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).AtMost(1).ComplyWith(x => x.IsLessThan(3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is less than 3 for at most one item,
				             but 2 of 5 were

				             Matching items:
				             [1, 2]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task AtMost_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).AtMost(1).Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for at most one item,
				             but 2 of 5 did

				             Matching items:
				             [1, 2]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task Between_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).Between(3).And(4).Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for between 3 and 4 items,
				             but only 2 of 5 did

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task Exactly_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).Exactly(1).Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for exactly one item,
				             but 2 of 5 did

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task LessThan_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).LessThan(2).Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for fewer than 2 items,
				             but 2 of 5 did

				             Matching items:
				             [1, 2]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task None_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).None().Satisfy(x => x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 3 for no items,
				             but 2 of 5 did

				             Matching items:
				             [1, 2]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}
	}

	public sealed class CollectionContextTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task DoesNotContain_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).DoesNotContain(2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 2,
				             but it contained 2 at least once

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasItem_AtIndex_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasItem(9).AtIndex(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 9 at index 1,
				             but it had item 2 at index 1

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasItem_AtIndexFromEnd_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasItem(9).AtIndexFromEnd(0);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 9 at index 0 from end,
				             but it had item 5 at index 0 from end

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasItemThat_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(9)).AtIndex(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to 9 at index 1,
				             but it had item 2 at index 1

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasSingle_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasSingle();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has a single item,
				             but it had more than one item

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task IsInAscendingOrder_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 1, 3, 3);

			async Task Act()
				=> await That(subject).IsInAscendingOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it had 2 before 1, which is not in ascending order

				             Collection:
				             [1, 2, 1, 3, 3]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task StartsWith_ShouldListAllItems(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).StartsWith(1, 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with [1, 3],
				             but it contained item 2 at index 1 instead of 3

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}
	}

	public sealed class EvaluationTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_Satisfy_WhenCanceledDuringTheEvaluation_ShouldStillVerifyAllItems(Kind kind)
		{
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			bool CancelAndVerify(int value)
			{
				cts.Cancel();
				return value < 6;
			}

			async Task Act()
				=> await That(subject).All().Satisfy(x => CancelAndVerify(x)).WithCancellation(token);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task All_Satisfy_WhenFailing_ShouldEnumerateAsOftenAsAnICollection()
		{
			CountingReadOnlyCollection<int> readOnlyCollection = new(1, 2, 3, 4, 5);
			CountingCollection<int> collection = new(1, 2, 3, 4, 5);
			IEnumerable<int> subject = readOnlyCollection;
			IEnumerable<int> other = collection;

			async Task Act()
				=> await That(subject).All().Satisfy(x => x < 3);

			async Task ActOnOther()
				=> await That(other).All().Satisfy(x => x < 3);

			await That(Act).Throws<FailException>();
			await That(ActOnOther).Throws<FailException>();
			await That(readOnlyCollection.Enumerations).IsEqualTo(2)
				.Because("the items are verified and then listed in the failure message");
			await That(collection.Enumerations).IsEqualTo(2);
		}

		[Test]
		public async Task HasCount_ShouldNotEnumerate()
		{
			CountingReadOnlyCollection<int> readOnlyCollection = new(1, 2, 3, 4, 5);
			IEnumerable<int> subject = readOnlyCollection;

			async Task Act()
				=> await That(subject).HasCount(5);

			await That(Act).DoesNotThrow();
			await That(readOnlyCollection.Enumerations).IsEqualTo(0);
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task HasCount_WhenTheCollectionDoesNotKnowItsNumberOfItems_ShouldStopAtTheDecidingItem()
		{
			IEnumerable<int> subject = ImmutableQueue.Create(1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it had at least 4 items

				             Collection:
				             [1, 2, 3, 4, (… and maybe more)]
				             """);
		}
#endif

		[Test]
		public async Task HasCount_WhenTheItemsAreReadAsTheirBaseType_ShouldReportTheNumberOfItems()
		{
			IEnumerable<object> subject = new List<string>
			{
				"a",
				"b",
				"c",
			};

			async Task Act()
				=> await That(subject).HasCount(2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 2 items,
				             but it had 3 items

				             Collection:
				             [
				               "a",
				               "b",
				               "c"
				             ]
				             """);
		}

		[Test]
		public async Task HasCount_WhenTheSubjectIsLazy_ShouldStopAtTheDecidingItem()
		{
			IEnumerable<int> subject = Lazy(1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it had at least 4 items

				             Collection:
				             [1, 2, 3, 4, (… and maybe more)]
				             """);
		}

		[Test]
		public async Task HasItem_AtIndexFromEnd_WhenTheSubjectAlsoCountsItemsOfAnotherType_ShouldCountTheItemsThatAreRead()
		{
			IEnumerable<int> subject = new CollectionOfTwoItemTypes([1, 2, 3,], ["a",]);

			async Task Act()
				=> await That(subject).HasItem(3).AtIndexFromEnd(0);

			await That(Act).DoesNotThrow()
				.Because("the index is counted from the end of the 3 items of the subject");
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasItem_AtIndexFromEnd_WhenCanceled_ShouldStillVerifyTheItem(Kind kind)
		{
			CancellationToken token = new(true);
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasItem(5).AtIndexFromEnd(0).WithCancellation(token);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task HasItemThat_AtIndexFromEnd_WhenTheSubjectAlsoCountsItemsOfAnotherType_ShouldCountTheItemsThatAreRead()
		{
			IEnumerable<int> subject = new CollectionOfTwoItemTypes([1, 2, 3,], ["a",]);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(3)).AtIndexFromEnd(0);

			await That(Act).DoesNotThrow()
				.Because("the index is counted from the end of the 3 items of the subject");
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task None_Satisfy_ShouldVerifyEveryItem(Kind kind)
		{
			IEnumerable<int> subject = Create(kind, 1, 2, 3, 4, 5);
			int verifiedItems = 0;

			bool CountAndVerify(int value)
			{
				verifiedItems++;
				return value < 3;
			}

			async Task Act()
				=> await That(subject).None().Satisfy(x => CountAndVerify(x));

			await That(Act).Throws<FailException>();
			await That(verifiedItems).IsEqualTo(5);
		}
	}

	public sealed class ExpectedTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task IsEqualTo_ShouldListTheExpectedItemsLikeTheSubject(Kind kind)
		{
			int[] subject = [1, 2, 3, 4, 5,];
			IEnumerable<int> expected = Create(kind, 1, 2, 3);

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it
				               contained item 4 at index 3 that was not expected and
				               contained item 5 at index 4 that was not expected

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [1, 2, 3]
				             """);
		}
	}

	public sealed class UntypedTests
	{
		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task All_Satisfy_ShouldCountAllItems(Kind kind)
		{
			IEnumerable subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).All().Satisfy(x => (int?)x < 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => (int?)x < 3 for all items,
				             but only 2 of 5 did

				             Not matching items:
				             [3, 4, 5]

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}

		[Test]
		[Arguments(Kind.List)]
		[Arguments(Kind.Queue)]
		[Arguments(Kind.Stack)]
		[Arguments(Kind.ReadOnlyCollection)]
		[Arguments(Kind.ConcurrentQueue)]
		public async Task HasCount_ShouldReportTheNumberOfItems(Kind kind)
		{
			IEnumerable subject = Create(kind, 1, 2, 3, 4, 5);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it had 5 items

				             Collection:
				             [1, 2, 3, 4, 5]
				             """);
		}
	}
}
