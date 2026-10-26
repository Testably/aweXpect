using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed class SignalerPredicateOptionsTests
{
	[Test]
	public async Task AddPredicate_ShouldNotChangeTheOriginalOptions()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;

		SignalerPredicateOptions<int> result = sut.AddPredicate(x => x > 1, "x => x > 1");

		await That(result.Matches(1)).IsFalse();
		await That(sut.Matches(1)).IsTrue();
		await That(sut.ToString()).IsEmpty();
		await That(SignalerPredicateOptions<int>.Default.ToString()).IsEmpty();
	}

	[Test]
	public async Task Matches_WhenAllPredicatesMatch_ShouldReturnTrue()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;
		sut = sut.AddPredicate(x => x > 1, "x => x > 1");
		sut = sut.AddPredicate(x => x < 3, "x => x < 3");

		bool result = sut.Matches(2);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Matches_WhenOnePredicateDoesNotMatch_ShouldReturnFalse()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;
		sut = sut.AddPredicate(x => x > 1, "x => x > 1");
		sut = sut.AddPredicate(x => x < 3, "x => x < 3");

		bool result = sut.Matches(3);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Matches_WithoutPredicate_ShouldReturnTrue()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;

		bool result = sut.Matches(42);

		await That(result).IsTrue();
	}

	[Test]
	public async Task ToString_Empty_ShouldReturnEmptyString()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task ToString_WhenPredicateIsAddedAfterFirstCall_ShouldAppendIt()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;
		sut = sut.AddPredicate(_ => true, "my predicate");
		_ = sut.ToString();
		sut = sut.AddPredicate(_ => true, "my other predicate");

		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate and with my other predicate");
	}

	[Test]
	public async Task ToString_WithPredicate_ShouldIncludePredicateExpression()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;
		sut = sut.AddPredicate(_ => true, "my predicate");

		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate");
	}

	[Test]
	public async Task ToString_WithPredicate_WhenCalledTwice_ShouldReturnSameText()
	{
		SignalerPredicateOptions<int> sut = SignalerPredicateOptions<int>.Default;
		sut = sut.AddPredicate(_ => true, "my predicate");

		_ = sut.ToString();
		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate");
	}
}
