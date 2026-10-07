using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class CastingEnumerableTests
{
	[Test]
	public async Task Count_WhenSourceIsACollection_ShouldBeItsCount()
	{
		CastingEnumerable<int, int?> enumerable = new(new List<int>
		{
			1,
			2,
			3,
		});

		await That(enumerable.Count).IsEqualTo(3);
	}

	[Test]
	public async Task Count_WhenSourceIsAReadOnlyCollection_ShouldBeItsCount()
	{
		CastingEnumerable<int, int?> enumerable = new(new ReadOnlyItems(1, 2));

		await That(enumerable.Count).IsEqualTo(2);
	}

	[Test]
	public async Task Count_WhenSourceIsCountable_ShouldFollowItsCount()
	{
		IEnumerable<int> source = MaterializingEnumerable<int>.WrapParameter(ToEnumerable([1, 2,]));
		CastingEnumerable<int, int?> enumerable = new(source);

		int? countBeforeEnumeration = enumerable.Count;
		_ = source.ToList();

		await That(countBeforeEnumeration).IsNull()
			.Because("the source does not know its count before it is enumerated");
		await That(enumerable.Count).IsEqualTo(2)
			.Because("the count is read from the source each time");
	}

	[Test]
	public async Task Count_WhenSourceIsNotCountable_ShouldBeNull()
	{
		CastingEnumerable<int, int?> enumerable = new(ToEnumerable([1, 2,]));

		await That(enumerable.Count).IsNull()
			.Because("finding the count would enumerate the source");
	}

	[Test]
	public async Task WhenEnumerated_ShouldCastEachItemOfTheSourceOnEveryEnumeration()
	{
		int readCount = 0;

		IEnumerable<int> GetSource()
		{
			readCount++;
			yield return 1;
			yield return 2;
		}

		CastingEnumerable<int, int?> enumerable = new(GetSource());

		int readCountBeforeEnumeration = readCount;
		List<int?> first = enumerable.ToList();
		List<int?> second = enumerable.ToList();

		await That(readCountBeforeEnumeration).IsEqualTo(0)
			.Because("the items are cast lazily like Enumerable.Cast");
		await That(readCount).IsEqualTo(2)
			.Because("every enumeration reads the source again, as Enumerable.Cast does");
		await That(first).IsEqualTo([1, 2,]);
		await That(second).IsEqualTo([1, 2,]);
	}

	[Test]
	public async Task WhenSourceIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new CastingEnumerable<int, int?>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("source").And
			.WithMessage(new ArgumentNullException("source").Message)
			.Because("a missing source is rejected right away, as Enumerable.Cast does");
	}

	private static IEnumerable<T> ToEnumerable<T>(T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}

	private sealed class ReadOnlyItems(params int[] values) : IReadOnlyCollection<int>
	{
		public int Count => values.Length;

		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)values).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
