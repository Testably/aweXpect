using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class CollectionCountResultTests
{
	[Test]
	public async Task NotGreaterThan_ShouldNegateTheQuantifierOfTheUnexpectedCount()
	{
		CollectionCountResult<(EnumerableQuantifier Quantifier, bool IsNegated)> sut = CreateSut();

		(EnumerableQuantifier quantifier, bool isNegated) = sut.NotGreaterThan(unexpected: 3);

		await That(quantifier.ToString()).IsEqualTo("more than 3");
		await That(isNegated).IsTrue();
	}

	[Test]
	public async Task NotGreaterThanOrEqualTo_ShouldNegateTheQuantifierOfTheUnexpectedCount()
	{
		CollectionCountResult<(EnumerableQuantifier Quantifier, bool IsNegated)> sut = CreateSut();

		(EnumerableQuantifier quantifier, bool isNegated) = sut.NotGreaterThanOrEqualTo(unexpected: 3);

		await That(quantifier.ToString()).IsEqualTo("at least 3");
		await That(isNegated).IsTrue();
	}

	[Test]
	public async Task NotLessThan_ShouldNegateTheQuantifierOfTheUnexpectedCount()
	{
		CollectionCountResult<(EnumerableQuantifier Quantifier, bool IsNegated)> sut = CreateSut();

		(EnumerableQuantifier quantifier, bool isNegated) = sut.NotLessThan(unexpected: 3);

		await That(quantifier.ToString()).IsEqualTo("fewer than 3");
		await That(isNegated).IsTrue();
	}

	[Test]
	public async Task NotLessThanOrEqualTo_ShouldNegateTheQuantifierOfTheUnexpectedCount()
	{
		CollectionCountResult<(EnumerableQuantifier Quantifier, bool IsNegated)> sut = CreateSut();

		(EnumerableQuantifier quantifier, bool isNegated) = sut.NotLessThanOrEqualTo(unexpected: 3);

		await That(quantifier.ToString()).IsEqualTo("at most 3");
		await That(isNegated).IsTrue();
	}

	private static CollectionCountResult<(EnumerableQuantifier Quantifier, bool IsNegated)> CreateSut()
		=> new((quantifier, isNegated) => (quantifier, isNegated));
}
