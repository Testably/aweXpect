global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static Player player = new();
}

// Stands in for the test context of xUnit v3.
internal class TestContext
{
	public static TestContext Current { get; } = new();
	public CancellationToken CancellationToken { get; }
}
