namespace Snippets;

// Stands in for the test context of xUnit v3.
internal class TestContext
{
	public static TestContext Current { get; } = new();
	public CancellationToken CancellationToken { get; }
}
