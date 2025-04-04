namespace aweXpect.Reflection.Tests;

public sealed class DummyTests
{
	[Fact]
	public async Task WhenPathIsAbsolute_ShouldSucceed()
	{
		string path = "/foo";

		async Task Act()
			=> await That(path).IsAbsolutePath();

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenPathIsRelative_ShouldFail()
	{
		string path = "foo";

		async Task Act()
			=> await That(path).IsAbsolutePath();

		await That(Act).ThrowsException()
			.WithMessage("""
			             Expected that path
			             is an absolute path,
			             but it was "foo"
			             """);
	}
}
