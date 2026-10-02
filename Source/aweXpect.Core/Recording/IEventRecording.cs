using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Recording;

/// <summary>
///     A recording of events on a subject of type <typeparamref name="TSubject" />.
/// </summary>
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IEventRecording<TSubject>
{
	/// <summary>
	///     Waits until the recorded events satisfy <paramref name="areFound" />, the <paramref name="timeout" />
	///     elapsed or the <paramref name="cancellationToken" /> was canceled, and returns the events recorded until
	///     then. The recording of events stops when the evaluation of the <paramref name="context" /> ends, unless it
	///     was set to <see cref="RecordExtensions.UntilDisposed{TSubject}(IEventRecording{TSubject})" />.
	/// </summary>
	/// <remarks>
	///     <paramref name="areFound" /> is checked initially and after each recorded event, for at most the
	///     <paramref name="timeout" />. A <paramref name="timeout" /> that is not positive does not wait at all, except
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" />, which waits without a limit.<br />
	///     A cancellation ends the wait like the <paramref name="timeout" /> does, so the result describes the events
	///     recorded until then.<br />
	///     All constraints of one expectation share the <paramref name="context" /> of its evaluation, so each of them
	///     can wait for the events that arrive during its own <paramref name="timeout" />. With any other
	///     <paramref name="context" />, or without one, the recording stops when the wait ends, and only that same
	///     non-<see langword="null" /> <paramref name="context" /> can check it again.
	/// </remarks>
	Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
		IEvaluationContext? context = null, CancellationToken cancellationToken = default);
}
#pragma warning restore S2326
