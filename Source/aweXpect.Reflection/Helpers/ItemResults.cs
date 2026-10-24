using aweXpect.Core.Extending;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The results of the generated expectations on the items of a collection.
/// </summary>
internal static class ItemResults
{
	/// <summary>
	///     A <see langword="null" /> item cannot be inspected, so it fails the expectation and its negation, like a
	///     <see langword="null" /> subject.
	/// </summary>
	public static PredicateResult Null => PredicateResult.FailureBothWays("was <null>");
}
