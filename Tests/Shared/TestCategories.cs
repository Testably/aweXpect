namespace aweXpect.TestHelpers;

/// <summary>
///     The categories that select tests in the build pipeline.
/// </summary>
public static class TestCategories
{
	/// <summary>
	///     Tests that take so long that they only run on request: locally through a filter, and in the pipeline in a
	///     step of their own. Every test of this category must also be <c>[Explicit]</c>.
	/// </summary>
	public const string Slow = "Slow";
}
