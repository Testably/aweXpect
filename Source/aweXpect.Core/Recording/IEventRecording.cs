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
	///     Stops the recording of events when checked events <see paramref="areFound" />
	///     or the <paramref name="timeout" /> elapsed.
	/// </summary>
	/// <remarks>
	///     A recording that was stopped with the same <paramref name="context" /> can be checked again, so that all
	///     constraints of one expectation describe the same snapshot.
	/// </remarks>
	Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
		IEvaluationContext? context = null);
}
#pragma warning restore S2326
