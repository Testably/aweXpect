using System;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Internal;

/// <summary>
///     The clock of an evaluation, on which its waits, its timeouts and the measured durations elapse.
/// </summary>
/// <remarks>
///     The types in this namespace are infrastructure, e.g. for a virtual clock in tests, and may change in any version.
///     <para />
///     An implementation is called concurrently and has to
///     <list type="bullet">
///         <item>return timestamps that never decrease, and that are only meaningful for the same instance,</item>
///         <item>complete a <see cref="Delay" /> not before its delay elapsed, and cancel it with its token,</item>
///         <item>
///             cancel a source of <see cref="CancelAfter" /> when its delay elapsed, at once for a delay of zero or less,
///             never for <see cref="Timeout.InfiniteTimeSpan" />, and not at all when the source is disposed before.
///         </item>
///     </list>
///     Waits and timeouts are separate members, unlike in <c>TimeProvider</c>, so that a virtual clock can jump to the
///     end of a wait while it only schedules a timeout.
/// </remarks>
public interface ITimeSystem
{
	/// <summary>
	///     Gets the current timestamp, from which <see cref="GetElapsedTime(long)" /> measures.
	/// </summary>
	long GetTimestamp();

	/// <summary>
	///     Gets the time elapsed since the <paramref name="startTimestamp" /> from <see cref="GetTimestamp()" />.
	/// </summary>
	TimeSpan GetElapsedTime(long startTimestamp);

	/// <summary>
	///     Creates a task that completes after the <paramref name="delay" />, or is canceled with the
	///     <paramref name="cancellationToken" />.
	/// </summary>
	/// <remarks>Like <see cref="Task.Delay(TimeSpan, CancellationToken)" />.</remarks>
	Task Delay(TimeSpan delay, CancellationToken cancellationToken);

	/// <summary>
	///     Cancels the <paramref name="cancellationTokenSource" /> after the <paramref name="delay" />.
	/// </summary>
	/// <remarks>Like <see cref="CancellationTokenSource.CancelAfter(TimeSpan)" />.</remarks>
	void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay);
}
