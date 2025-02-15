namespace aweXpect.Web.Tests;

public sealed class DummyTests
{
	[Fact]
	public async Task MyDummyTest()
	{
		bool sut = true;

		await That(sut).IsTrue();
	}
}
