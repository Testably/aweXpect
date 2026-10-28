using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     Options that apply to every evaluation of an expectation.
/// </summary>
public static class ExpectationExtensions
{
	/// <summary>
	///     Lets every evaluation of the <paramref name="expectation" /> run on the <paramref name="timeProvider" />:
	///     its waits, e.g. of <c>Within</c> or <c>Eventually</c>, its timeouts and the durations it measures.
	/// </summary>
	/// <remarks>
	///     For a combination, e.g. of <c>Expect.ThatAll</c>, it applies to every combined expectation.<br />
	///     The waits and timeouts are timers of the <paramref name="timeProvider" />, so on a provider that only
	///     moves when the test advances it, e.g. a <c>FakeTimeProvider</c>, they only elapse when the test advances it.
	///     Only a timeout of <c>TestCancellation.FromTimeout</c> elapses on the <paramref name="timeProvider" />, not
	///     the token of <c>TestCancellation.FromCancellationToken</c>, and code that waits outside the evaluation, e.g.
	///     a subject, waits on its own clock.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	///     The <paramref name="expectation" /> or the <paramref name="timeProvider" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">A time provider is already set.</exception>
	public static TExpectation WithTimeProvider<TExpectation>(this TExpectation expectation,
		TimeProvider timeProvider)
		where TExpectation : Expectation
	{
		expectation.ThrowIfNull();
		timeProvider.ThrowIfNull();
		expectation.UseTimeProvider(timeProvider);
		return expectation;
	}
}
