using System;
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
	///     Waits until the recorded events satisfy <paramref name="areFound" /> or the <paramref name="timeout" />
	///     elapsed, and then stops the recording of events, unless it was set to
	///     <see cref="RecordExtensions.UntilDisposed{TSubject}(IEventRecording{TSubject})" />.
	/// </summary>
	/// <remarks>
	///     <paramref name="areFound" /> is checked initially and after each recorded event, for at most the
	///     <paramref name="timeout" />. A <paramref name="timeout" /> that is not positive does not wait at all.<br />
	///     A recording that was stopped with the same <paramref name="context" /> can be checked again, so that all
	///     constraints of one expectation describe the same snapshot.
	/// </remarks>
	Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
		IEvaluationContext? context = null);
}
#pragma warning restore S2326
