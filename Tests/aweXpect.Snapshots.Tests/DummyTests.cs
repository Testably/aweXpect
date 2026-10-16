namespace aweXpect.Snapshots.Tests;

public sealed class DummyTests
{
	[Fact]
	public async Task Dummy_ShouldSucceed()
		=> await That(true).IsTrue();
}
