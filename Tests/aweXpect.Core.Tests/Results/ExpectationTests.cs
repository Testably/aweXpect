using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public class ExpectationTests
{
	[Test]
	public async Task Equals_ShouldThrowNotSupportedException()
	{
#pragma warning disable aweXpect0001, aweXpect0002
		Expectation sut = That(true).IsTrue();

		bool Act() => sut.Equals(That(true).IsTrue());
#pragma warning restore aweXpect0001, aweXpect0002

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Equals is not supported. Did you mean IsEqualTo() instead?");
	}

	[Test]
	public async Task GetHashCode_ShouldThrowNotSupportedException()
	{
#pragma warning disable aweXpect0001
		Expectation sut = That(true).IsTrue();
#pragma warning restore aweXpect0001

		int Act() => sut.GetHashCode();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("GetHashCode is not supported.");
	}

	[Test]
	public async Task GetType_ShouldForwardToBase()
	{
#pragma warning disable aweXpect0001
		Expectation sut = That(true).IsTrue();
#pragma warning restore aweXpect0001

		Type type = sut.GetType();

		await That(type).IsEqualTo(typeof(AndOrResult<bool, IThat<bool>>));
	}

	[Test]
	public async Task ToString_ShouldForwardToExpectationBuilder()
	{
#pragma warning disable aweXpect0001
		Expectation sut = That(1).IsGreaterThan(0);
#pragma warning restore aweXpect0001

		string? result = sut.ToString();

		await That(result)
			.IsEqualTo("aweXpect.Core.ExpectationBuilder`1[System.Int32]");
	}
}
