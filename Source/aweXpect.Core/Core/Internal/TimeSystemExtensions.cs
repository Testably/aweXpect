using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Internal;

/// <summary>
///     Lets expectations run on another <see cref="ITimeSystem" />.
/// </summary>
/// <remarks>
///     The types in this namespace are infrastructure, e.g. for a virtual clock in tests, and may change in any version.
/// </remarks>
public static class TimeSystemExtensions
{
	/// <summary>
	///     Lets every evaluation of the <paramref name="expectation" /> run on the <paramref name="timeSystem" />: its
	///     waits, e.g. of <c>Within</c> or <c>Eventually</c>, its timeouts and the durations it measures.
	/// </summary>
	/// <remarks>
	///     For a combination, e.g. of <c>Expect.ThatAll</c>, it applies to every combined expectation.<br />
	///     Only a timeout of <c>TestCancellation.FromTimeout</c> elapses on the <paramref name="timeSystem" />, not the
	///     token of <c>TestCancellation.FromCancellationToken</c>, and code that waits outside the evaluation, e.g. a
	///     subject, still waits in real time.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	///     The <paramref name="expectation" /> or the <paramref name="timeSystem" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">A time system is already set.</exception>
	public static TExpectation WithTimeSystem<TExpectation>(this TExpectation expectation, ITimeSystem timeSystem)
		where TExpectation : Expectation
	{
		expectation.ThrowIfNull();
		timeSystem.ThrowIfNull();
		expectation.UseTimeSystem(timeSystem);
		return expectation;
	}
}
