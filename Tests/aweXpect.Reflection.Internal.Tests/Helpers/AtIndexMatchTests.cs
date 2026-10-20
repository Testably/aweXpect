using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class AtIndexMatchTests
{
	[Test]
	public async Task Constructor_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act()
		{
			_ = new AtIndexMatch(-1);
		}

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The index must be greater than or equal to 0.*").AsWildcard();
	}

	[Test]
	public async Task FromEnd_GetDescription_ShouldAppendFromEnd()
	{
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(2).FromEnd();

		await That(sut.GetDescription()).IsEqualTo(" at index 2 from end");
	}

	[Test]
	public async Task FromEnd_MatchesIndex_AfterExpectedIndexFromEnd_ShouldReturnFalse()
	{
		// _index = 1, count = 5 => expected = 3
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(1).FromEnd();

		await That(sut.MatchesIndex(4, 5)).IsEqualTo(false);
	}

	[Test]
	public async Task FromEnd_MatchesIndex_AtExpectedIndexFromEnd_ShouldReturnTrue()
	{
		// _index = 1, count = 5 => expected = 5 - 1 - 1 = 3
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(1).FromEnd();

		await That(sut.MatchesIndex(3, 5)).IsEqualTo(true);
	}

	[Test]
	public async Task FromEnd_MatchesIndex_BeforeExpectedIndexFromEnd_ShouldReturnNull()
	{
		// _index = 1, count = 5 => expected = 3
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(1).FromEnd();

		await That(sut.MatchesIndex(2, 5)).IsNull();
	}

	[Test]
	public async Task FromEnd_MatchesIndex_WhenCountIsNull_ShouldReturnNull()
	{
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(2).FromEnd();

		await That(sut.MatchesIndex(0, null)).IsNull();
	}

	[Test]
	public async Task FromEnd_OnlySingleIndex_ShouldReturnTrue()
	{
		CollectionIndexOptions.IMatchFromEnd sut = new AtIndexMatch(2).FromEnd();

		await That(sut.OnlySingleIndex()).IsTrue();
	}

	[Test]
	public async Task GetDescription_ShouldIncludeIndex()
	{
		AtIndexMatch sut = new(3);

		await That(sut.GetDescription()).IsEqualTo(" at index 3");
	}

	[Test]
	public async Task MatchesIndex_AfterIndex_ShouldReturnFalse()
	{
		AtIndexMatch sut = new(2);

		await That(sut.MatchesIndex(3)).IsEqualTo(false);
	}

	[Test]
	public async Task MatchesIndex_AtIndex_ShouldReturnTrue()
	{
		AtIndexMatch sut = new(2);

		await That(sut.MatchesIndex(2)).IsEqualTo(true);
	}

	[Test]
	public async Task MatchesIndex_WhenBeforeIndex_ShouldReturnNull()
	{
		AtIndexMatch sut = new(2);

		await That(sut.MatchesIndex(1)).IsNull();
	}

	[Test]
	public async Task OnlySingleIndex_ShouldReturnTrue()
	{
		AtIndexMatch sut = new(2);

		await That(sut.OnlySingleIndex()).IsTrue();
	}
}
