using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionOrderOptionsTests
{
	[Fact]
	public async Task SetComparer_WithNull_ShouldThrowArgumentNullException()
	{
		CollectionOrderOptions<int> sut = new();

		void Act() => sut.SetComparer(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
	}

	[Fact]
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
