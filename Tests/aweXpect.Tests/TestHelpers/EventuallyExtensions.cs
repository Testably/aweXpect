using aweXpect.Delegates;

namespace aweXpect.Tests;

internal static class EventuallyExtensions
{
	/// <summary>
	///     Retries once: the subject is evaluated at once and again when the <paramref name="timeout" /> is used up.
	/// </summary>
	/// <remarks>
	///     An attempt that takes longer than its limit is reported as not finished, whatever it observed. The limit is
	///     the remaining timeout, but at least one check interval, so with a short interval the last attempt, which is
	///     made when the timeout is used up, only has that interval. With the timeout as interval both attempts have the
	///     whole timeout, which leaves a busy machine enough time for a subject that returns at once.
	/// </remarks>
	public static EventuallySubject<T> WithinTwoAttempts<T>(this EventuallySubject<T> subject, TimeSpan timeout)
		=> subject.Within(timeout).CheckEvery(timeout);
}
