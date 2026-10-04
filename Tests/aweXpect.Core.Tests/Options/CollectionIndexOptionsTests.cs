using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionIndexOptionsTests
{
	[Fact]
	public async Task AtIndex_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		void Act() => sut.AtIndex(2);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndex cannot be specified more than once.");
	}

	[Fact]
	public async Task AtIndexFromEnd_ShouldMatchTheIndexCountedFromTheEnd()
	{
		CollectionIndexOptions sut = new();

		sut.AtIndexFromEnd(1);

		await That(sut.Match.GetDescription()).IsEqualTo(" at index 1 from end");
		await That(sut.Match).Is<CollectionIndexOptions.IMatchFromEnd>()
			.Whose(match => match.MatchesIndex(3, 5), it => it.IsTrue());
	}

	[Fact]
	public async Task AtIndexFromEnd_WhenAtIndexIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut.AtIndex(1);

		void Act() => sut.AtIndexFromEnd(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndexFromEnd cannot be combined with AtIndex.");
	}

	[Fact]
	public async Task DefaultMatch_FromEnd_ShouldThrowNotSupportedException()
	{
		CollectionIndexOptions sut = new();
		CollectionIndexOptions.IMatchFromBeginning match = (CollectionIndexOptions.IMatchFromBeginning)sut.Match;

		void Act()
			=> _ = match.FromEnd();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("You have to specify a dedicated index condition first.");
	}

	[Fact]
	public async Task DefaultMatch_ShouldHaveExpectedReturnValues()
	{
		CollectionIndexOptions sut = new();

		await That(sut.Match.GetDescription()).IsEmpty();
		await That(sut.Match.OnlySingleIndex()).IsFalse();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(3)]
	[InlineData(-1)]
	public async Task DefaultMatch_ShouldMatchAllIndices(int index)
	{
		CollectionIndexOptions sut = new();
		CollectionIndexOptions.IMatchFromBeginning match = (CollectionIndexOptions.IMatchFromBeginning)sut.Match;

		await That(match.MatchesIndex(index)).IsTrue();
	}
}
