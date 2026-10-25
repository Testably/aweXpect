using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed class SignalerPredicateOptionsTests
{
	[Test]
	public async Task Matches_WhenAllPredicatesMatch_ShouldReturnTrue()
	{
		SignalerPredicateOptions<int> sut = new();
		sut.WithPredicate(x => x > 1, "x => x > 1");
		sut.WithPredicate(x => x < 3, "x => x < 3");

		bool result = sut.Matches(2);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Matches_WhenOnePredicateDoesNotMatch_ShouldReturnFalse()
	{
		SignalerPredicateOptions<int> sut = new();
		sut.WithPredicate(x => x > 1, "x => x > 1");
		sut.WithPredicate(x => x < 3, "x => x < 3");

		bool result = sut.Matches(3);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Matches_WithoutPredicate_ShouldReturnTrue()
	{
		SignalerPredicateOptions<int> sut = new();

		bool result = sut.Matches(42);

		await That(result).IsTrue();
	}

	[Test]
	public async Task ToString_Empty_ShouldReturnEmptyString()
	{
		SignalerPredicateOptions<int> sut = new();

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task ToString_WhenPredicateIsAddedAfterFirstCall_ShouldAppendIt()
	{
		SignalerPredicateOptions<int> sut = new();
		sut.WithPredicate(_ => true, "my predicate");
		_ = sut.ToString();
		sut.WithPredicate(_ => true, "my other predicate");

		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate and with my other predicate");
	}

	[Test]
	public async Task ToString_WithPredicate_ShouldIncludePredicateExpression()
	{
		SignalerPredicateOptions<int> sut = new();
		sut.WithPredicate(_ => true, "my predicate");

		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate");
	}

	[Test]
	public async Task ToString_WithPredicate_WhenCalledTwice_ShouldReturnSameText()
	{
		SignalerPredicateOptions<int> sut = new();
		sut.WithPredicate(_ => true, "my predicate");

		_ = sut.ToString();
		string result = sut.ToString();

		await That(result).IsEqualTo(" with my predicate");
	}

	[Test]
	public async Task WithPredicate_ShouldReturnTheSameOptions()
	{
		SignalerPredicateOptions<int> sut = new();

		SignalerPredicateOptions<int> result = sut.WithPredicate(_ => true, "my predicate");

		await That(result).IsSameAs(sut);
	}
}
