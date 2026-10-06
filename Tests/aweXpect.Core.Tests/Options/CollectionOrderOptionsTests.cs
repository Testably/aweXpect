using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionOrderOptionsTests
{
	[Test]
	[Arguments(1, 2, -1)]
	[Arguments(2, 1, 1)]
	[Arguments(null, 1, -1)]
	public async Task GetComparer_ForObjects_ShouldCompareOtherItemsWithTheDefaultComparer(
		object? x, object? y, int expected)
	{
		CollectionOrderOptions<object?> sut = new();

		int result = Math.Sign(sut.GetComparer().Compare(x, y));

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments("a", "B", 1)]
	[Arguments("B", "a", -1)]
	[Arguments("a", "a", 0)]
	public async Task GetComparer_ForObjects_ShouldCompareStringsOrdinally(string x, string y, int expected)
	{
		CollectionOrderOptions<object?> sut = new();

		int result = Math.Sign(sut.GetComparer().Compare(x, y));

		await That(result).IsEqualTo(expected)
			.Because("strings are ordered ordinally, whatever the item type of the collection");
	}

	[Test]
	public async Task SetComparer_WithNull_ShouldThrowArgumentNullException()
	{
		CollectionOrderOptions<int> sut = new();

		void Act() => sut.SetComparer(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ToString_WithGenericComparer_ShouldFormatTheComparerType()
	{
		CollectionOrderOptions<int> sut = new();
		sut.SetComparer(new ReverseComparer<int>());

		string result = sut.ToString();

		await That(result).IsEqualTo(" using CollectionOrderOptionsTests.ReverseComparer<int>")
			.Because("the comparer is named like in every other expectation");
	}

	private sealed class ReverseComparer<T> : IComparer<T>
	{
		public int Compare(T? x, T? y) => Comparer<T>.Default.Compare(y!, x!);
	}
}
