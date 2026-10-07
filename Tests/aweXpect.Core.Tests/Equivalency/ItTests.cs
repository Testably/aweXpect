using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class ItTests
{
	[Test]
	public async Task Is_ToString_ShouldDescribeTheExpectedType()
	{
		It.IsEquivalent<int> sut = It.Is<int>();

		string? result = sut.ToString();

		await That(result).IsEqualTo("is int");
	}
}
