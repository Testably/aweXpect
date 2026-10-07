using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionIndexOptionsTests
{
	[Test]
	public async Task AtIndex_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		void Act() => sut.AtIndex(2);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndex cannot be specified more than once.");
	}

	[Test]
	public async Task AtIndexFromEnd_ShouldMatchTheIndexCountedFromTheEnd()
	{
		CollectionIndexOptions sut = new();

		sut.AtIndexFromEnd(1);

		await That(sut.Match.GetDescription()).IsEqualTo(" at index 1 from end");
		await That(sut.Match).Is<CollectionIndexOptions.IMatchFromEnd>()
			.Whose(match => match.MatchesIndex(3, 5), it => it.IsTrue());
	}

	[Test]
	public async Task AtIndexFromEnd_WhenAtIndexIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		void Act() => sut.AtIndexFromEnd(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndexFromEnd cannot be combined with AtIndex.");
	}

	[Test]
	public async Task DefaultMatch_FromEnd_ShouldThrowNotSupportedException()
	{
		CollectionIndexOptions sut = new();
		CollectionIndexOptions.IMatchFromBeginning match = (CollectionIndexOptions.IMatchFromBeginning)sut.Match;

		void Act()
			=> _ = match.FromEnd();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("You have to specify a dedicated index condition first.");
	}

	[Test]
	public async Task DefaultMatch_ShouldHaveExpectedReturnValues()
	{
		CollectionIndexOptions sut = new();

		await That(sut.Match.GetDescription()).IsEmpty();
		await That(sut.Match.OnlySingleIndex()).IsFalse();
	}

	[Test]
	[Arguments(0)]
	[Arguments(1)]
	[Arguments(3)]
	[Arguments(-1)]
	public async Task DefaultMatch_ShouldMatchAllIndices(int index)
	{
		CollectionIndexOptions sut = new();
		CollectionIndexOptions.IMatchFromBeginning match = (CollectionIndexOptions.IMatchFromBeginning)sut.Match;

		await That(match.MatchesIndex(index)).IsTrue();
	}

	[Test]
	public async Task SetMatch_ShouldReplaceASpecifiedIndex()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		sut.SetMatch(new MyMatch());

		await That(sut.Match).Is<MyMatch>()
			.Because("only the overload with the name of the option rejects a second index");
	}

	[Test]
	public async Task SetMatch_WithOption_ShouldSetTheMatch()
	{
		CollectionIndexOptions sut = new();

		sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(sut.Match).Is<MyMatch>();
	}

	[Test]
	public async Task SetMatch_WithOption_WhenAtIndexIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		void Act() => sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtMyIndex cannot be combined with AtIndex.");
		await That(sut.Match.GetDescription()).IsEqualTo(" at index 1");
	}

	[Test]
	public async Task SetMatch_WithOption_WhenFollowedByAtIndex_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.SetMatch(new MyMatch(), "AtMyIndex");

		void Act() => sut.AtIndex(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndex cannot be combined with AtMyIndex.");
		await That(sut.Match).Is<MyMatch>();
	}

	[Test]
	public async Task SetMatch_WithOption_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.SetMatch(new MyMatch(), "AtMyIndex");

		void Act() => sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtMyIndex cannot be specified more than once.");
	}

	private sealed class MyMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " at my index";

		public bool OnlySingleIndex() => false;
	}
}
