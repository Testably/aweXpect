using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Synchronous;

/// <summary>
///     Extension methods to support synchronous execution.
/// </summary>
/// <remarks>
///     <b>WARNING!</b><br />
///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
/// </remarks>
public static class SynchronouslyExtensions
{
	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
	/// </remarks>
	public static void VerifySynchronously<TSelf>(this ExpectationResultWithoutValue<TSelf> result)
		where TSelf : ExpectationResultWithoutValue<TSelf>
		=> Synchronously.Verify(result);

	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
	/// </remarks>
	public static TType VerifySynchronously<TType, TSelf>(this ExpectationResult<TType, TSelf> result)
		where TSelf : ExpectationResult<TType, TSelf>
		=> Synchronously.Verify(result);

	/// <summary>
	///     Verifies synchronously that the <paramref name="combination" /> of expectations is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     It blocks the thread until the expectation is evaluated, so only use it where awaiting is impossible, e.g.
///     for a <c>ref struct</c> such as <c>Span&lt;T&gt;</c>, inside a <c>lock</c> or in a synchronous callback.
	/// </remarks>
	public static void VerifySynchronously(this Expectation.Combination combination)
		=> Synchronously.Verify(combination);
}
