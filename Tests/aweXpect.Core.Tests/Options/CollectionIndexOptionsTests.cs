using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionIndexOptionsTests
{
	[Test]
	public async Task AtIndex_ShouldNotChangeTheOriginalOptions()
	{
		CollectionIndexOptions sut = CollectionIndexOptions.Default;

		CollectionIndexOptions result = sut.AtIndex(1);

		await That(result.Match.GetDescription()).IsEqualTo(" at index 1");
		await That(sut.Match.GetDescription()).IsEmpty()
			.Because("the options are an immutable value that can be shared");
	}

	[Test]
	public async Task AtIndex_WhenAtIndexFromEndIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndexFromEnd(1);

		void Act() => sut.AtIndex(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndex cannot be combined with AtIndexFromEnd.");
	}

	[Test]
	public async Task AtIndex_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		CollectionIndexOptions sut = new();

		void Act() => sut.AtIndex(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("index").And
			.WithMessage("The index must not be negative.").AsPrefix();
	}

	[Test]
	public async Task AtIndex_WhenSpecifiedTwice_ShouldUseTheLastIndex()
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndex(1);

		sut = sut.AtIndex(2);

		await That(sut.Match.GetDescription()).IsEqualTo(" at index 2");
	}

	[Test]
	public async Task AtIndexFromEnd_ShouldMatchTheIndexCountedFromTheEnd()
	{
		CollectionIndexOptions sut = new();

		sut = sut.AtIndexFromEnd(1);

		await That(sut.Match.GetDescription()).IsEqualTo(" at index 1 from end");
		await That(sut.Match).Is<CollectionIndexOptions.IMatchFromEnd>()
			.Whose(match => match.MatchesIndex(3, 5), it => it.IsTrue());
	}

	[Test]
	public async Task AtIndexFromEnd_ShouldNotChangeTheOriginalOptions()
	{
		CollectionIndexOptions sut = CollectionIndexOptions.Default.AtIndexFromEnd(2);

		CollectionIndexOptions result = sut.AtIndexFromEnd(1);

		await That(result.Match.GetDescription()).IsEqualTo(" at index 1 from end");
		await That(sut.Match.GetDescription()).IsEqualTo(" at index 2 from end")
			.Because("the options are an immutable value that can be shared");
	}

	[Test]
	public async Task AtIndexFromEnd_WhenAtIndexIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndex(1);

		void Act() => sut.AtIndexFromEnd(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndexFromEnd cannot be combined with AtIndex.");
	}

	[Test]
	public async Task AtIndexFromEnd_WhenCountIsUnknown_ShouldNotDecideTheMatch()
	{
		CollectionIndexOptions sut = new();

		sut = sut.AtIndexFromEnd(1);

		await That(sut.Match).Is<CollectionIndexOptions.IMatchFromEnd>()
			.Whose(match => match.MatchesIndex(3, null), it => it.IsNull());
	}

	[Test]
	public async Task AtIndexFromEnd_WhenIndexIsBeforeTheExpectedIndex_ShouldNotDecideTheMatch()
	{
		CollectionIndexOptions sut = new();

		sut = sut.AtIndexFromEnd(1);

		await That(sut.Match).Is<CollectionIndexOptions.IMatchFromEnd>()
			.Whose(match => match.MatchesIndex(2, 5), it => it.IsNull());
	}

	[Test]
	public async Task Default_ShouldMatchEveryIndex()
	{
		CollectionIndexOptions sut = default;

		await That(sut.Match.GetDescription()).IsEmpty();
		await That(sut.MatchesIndex(3, null)).IsEqualTo(true);
		await That(CollectionIndexOptions.Default.MatchesIndex(3, 5)).IsEqualTo(true);
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
	[Arguments(0, null)]
	[Arguments(1, true)]
	[Arguments(2, false)]
	public async Task MatchesIndex_WhenAtIndex_ShouldOnlyMatchTheIndex(int index, bool? expected)
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndex(1);

		bool? result = sut.MatchesIndex(index, 3);

		await That(result).IsEqualTo(expected)
			.Because("an earlier index could still be followed by the expected one, a later index cannot");
	}

	[Test]
	[Arguments(0, null)]
	[Arguments(1, true)]
	[Arguments(2, false)]
	public async Task MatchesIndex_WhenAtIndexFromEnd_ShouldCountFromTheEnd(int index, bool? expected)
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndexFromEnd(1);

		bool? result = sut.MatchesIndex(index, 3);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task MatchesIndex_WhenAtIndexFromEndAndCountIsUnknown_ShouldReturnNull()
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndexFromEnd(1);

		bool? result = sut.MatchesIndex(1, null);

		await That(result).IsNull();
	}

	[Test]
	public async Task MatchesIndex_WhenMatchIsNeitherFromBeginningNorFromEnd_ShouldReturnFalse()
	{
		CollectionIndexOptions sut = new();
		sut = sut.SetMatch(new MyMatch(), "AtMyIndex");

		bool? result = sut.MatchesIndex(0, 1);

		await That(result).IsEqualTo(false);
	}

	[Test]
	[Arguments(0)]
	[Arguments(5)]
	public async Task MatchesIndex_WithDefaultMatch_ShouldMatchAllIndices(int index)
	{
		CollectionIndexOptions sut = new();

		bool? result = sut.MatchesIndex(index, null);

		await That(result).IsEqualTo(true);
	}

	[Test]
	public async Task SetMatch_WithOption_ShouldNotChangeTheOriginalOptions()
	{
		CollectionIndexOptions sut = CollectionIndexOptions.Default;

		CollectionIndexOptions result = sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(result.Match).Is<MyMatch>();
		await That(sut.Match).IsNot<MyMatch>()
			.Because("the options are an immutable value that can be shared");
	}

	[Test]
	public async Task SetMatch_WithOption_ShouldSetTheMatch()
	{
		CollectionIndexOptions sut = new();

		sut = sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(sut.Match).Is<MyMatch>();
	}

	[Test]
	public async Task SetMatch_WithOption_WhenAtIndexIsSpecified_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut = sut.AtIndex(1);

		void Act() => sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtMyIndex cannot be combined with AtIndex.");
		await That(sut.Match.GetDescription()).IsEqualTo(" at index 1");
	}

	[Test]
	public async Task SetMatch_WithOption_WhenFollowedByAtIndex_ShouldThrowInvalidOperationException()
	{
		CollectionIndexOptions sut = new();
		sut = sut.SetMatch(new MyMatch(), "AtMyIndex");

		void Act() => sut.AtIndex(1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtIndex cannot be combined with AtMyIndex.");
		await That(sut.Match).Is<MyMatch>();
	}

	[Test]
	public async Task SetMatch_WithOption_WhenSpecifiedTwice_ShouldUseTheLastMatch()
	{
		CollectionIndexOptions sut = new();
		MyMatch lastMatch = new();
		sut = sut.SetMatch(new MyMatch(), "AtMyIndex");

		sut = sut.SetMatch(lastMatch, "AtMyIndex");

		await That(sut.Match).IsSameAs(lastMatch);
	}

	private sealed class MyMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " at my index";

		public bool OnlySingleIndex() => false;
	}
}
