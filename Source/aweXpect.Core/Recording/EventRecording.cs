using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;

namespace aweXpect.Recording;

internal sealed class EventRecording<TSubject> : IDisposableEventRecording<TSubject>, IEventRecordingResult
	where TSubject : notnull
{
	/// <remarks>
	///     A registered type is served from the <see cref="TypeMetadataRegistry" /> and its events are known
	///     completely, so a missing event is missing for sure. Reflection cannot tell a missing event from one that
	///     publishing with trimming or Native AOT enabled removed.
	/// </remarks>
	private const string TrimmingHint =
		". When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.";

	private readonly bool _isRegistered;
	private readonly List<EventRecorder> _recorders;

	/// <remarks>
	///     An event that the reflective fallback cannot bind a handler to must not cost the recording of all the other
	///     events of the subject, so it is skipped with its reason, which the expectation that asks for it reports
	///     instead of letting it look like an event that was never triggered.
	/// </remarks>
	private readonly Dictionary<string, string>? _skipped;

	private readonly string _subjectExpression;

	private bool _isStopped;

	/// <remarks>
	///     Created by the first waiter and completed and removed on the next recorded event, so that any number of
	///     concurrent waiters wake up. A waiter reads it before it checks the recorded events, so that an event in
	///     between completes the task it awaits.
	/// </remarks>
	private TaskCompletionSource<bool>? _recorded;

	private IEvaluationContext? _stoppedBy;
	private bool _stopsAfterEvaluation = true;

	/// <summary>
	///     Creates a new recording the given <paramref name="eventNames" /> that are triggered on the
	///     <paramref name="subject" />.
	/// </summary>
	public EventRecording(TSubject subject, string subjectExpression, params string[] eventNames)
	{
		_subjectExpression = subjectExpression;
		IRecordableEvent[]? registeredEvents = GetRegistered(subject.GetType());
		_isRegistered = registeredEvents is not null;
		IRecordableEvent[] events = registeredEvents ?? ReflectedEvent.GetAll(subject.GetType());

		bool recordAllEvents = eventNames.Length == 0;
		if (!recordAllEvents)
		{
			ThrowIfRequestedMoreThanOnce(eventNames);
		}

		_recorders = new List<EventRecorder>(recordAllEvents ? events.Length : eventNames.Length);
		Action notifyRecordedEvent = NotifyRecordedEvent;
		try
		{
			int count = recordAllEvents ? events.Length : eventNames.Length;
			for (int i = 0; i < count; i++)
			{
				IRecordableEvent @event = recordAllEvents ? events[i] : Find(events, eventNames[i], subject);
				string? unsupported = TryAttach(subject, @event, notifyRecordedEvent);
				if (unsupported is null)
				{
					continue;
				}

				if (recordAllEvents)
				{
					(_skipped ??= new Dictionary<string, string>()).Add(@event.Name, unsupported);
				}
				else
				{
					// An event that was asked for by name is what the recording is about, so it fails right away.
					throw Tracing.WriteException(new NotSupportedException(unsupported));
				}
			}
		}
		catch
		{
			// Nobody ever receives a recording whose construction threw, so the handlers it already attached would
			// stay on the subject for its lifetime with nothing left that could detach them.
			Stop(null);
			throw;
		}
	}

	public async Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
		IEvaluationContext? context = null, CancellationToken cancellationToken = default)
	{
		ThrowIfStopped(context);
		try
		{
			if (timeout > TimeSpan.Zero || timeout == Timeout.InfiniteTimeSpan)
			{
				await WaitUntil(areFound, timeout, cancellationToken);
			}
		}
		catch
		{
			if (_stopsAfterEvaluation)
			{
				// A predicate that throws must not leave the handlers attached to the subject.
				Stop(context);
			}

			throw;
		}

		Snapshot snapshot = new(this);
		if (_stopsAfterEvaluation)
		{
			StopWithEvaluation(context);
		}

		return snapshot;
	}

	/// <inheritdoc cref="IDisposable.Dispose()" />
	public void Dispose() => Stop(null);

	/// <summary>
	///     Gets the number of recorded events for <paramref name="eventName" /> that match the <paramref name="filter" />.
	/// </summary>
	public int GetEventCount(string eventName, Func<object?[], bool>? filter = null)
		=> _recorders[GetRecorderIndex(eventName)].GetEventCount(filter);

	/// <summary>
	///     Returns a formatted string for the recorded events for <paramref name="eventName" />.
	/// </summary>
	public string ToString(string eventName)
		=> _recorders[GetRecorderIndex(eventName)].ToString();

	/// <inheritdoc />
	public override string ToString()
		=> _subjectExpression;

	private IRecordableEvent Find(IRecordableEvent[] events, string eventName, TSubject subject)
	{
		foreach (IRecordableEvent @event in events)
		{
			if (@event.Name == eventName)
			{
				return @event;
			}
		}

		throw Tracing.WriteException(
			new NotSupportedException(
				$"Event {eventName} is not supported on {Formatter.Format(subject)}{(_isRegistered ? "." : TrimmingHint)}"));
	}

	/// <summary>
	///     Records the <paramref name="event" /> and returns the reason why it cannot be recorded, or
	///     <see langword="null" /> when the handler was attached.
	/// </summary>
	private string? TryAttach(TSubject subject, IRecordableEvent @event, Action notifyRecordedEvent)
	{
		EventRecorder recorder = new(@event.Name, notifyRecordedEvent);
		// Stored before it is attached, so that the cleanup in the constructor also detaches it when anything later
		// throws.
		_recorders.Add(recorder);
		string? unsupported = recorder.TryAttach(subject, @event);
		if (unsupported is not null)
		{
			_recorders.RemoveAt(_recorders.Count - 1);
		}

		return unsupported;
	}

	/// <remarks>
	///     A repeated name is rejected instead of ignored, because it is most likely a mistake for another event, which
	///     would otherwise silently not be recorded.
	/// </remarks>
	private static void ThrowIfRequestedMoreThanOnce(string[] eventNames)
	{
		for (int i = 0; i < eventNames.Length; i++)
		{
			if (Array.IndexOf(eventNames, eventNames[i], i + 1) >= 0)
			{
				throw Tracing.WriteException(new ArgumentException(
					$"Event {eventNames[i]} was requested more than once.", nameof(eventNames)));
			}
		}
	}

	/// <remarks>
	///     A type counts as registered only when it has an event: a registration of its members says nothing about the
	///     events, so such a type is reflected over like an unregistered one.
	/// </remarks>
	private static IRecordableEvent[]? GetRegistered(Type type)
		=> TypeMetadataRegistry.Instance.TryGet(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		   !metadata.Events.IsEmpty
			? metadata.OrderedEvents
			: null;

	/// <remarks>
	///     The later constraints of the same expectation can still wait for events, so the recording only stops when
	///     the evaluation ends. Without an <see cref="Core.EvaluationContext.EvaluationContext" /> of this library,
	///     nothing would ever end it, so it stops right away.
	/// </remarks>
	private void StopWithEvaluation(IEvaluationContext? context)
	{
		if (context is EvaluationContext evaluationContext)
		{
			evaluationContext.ReleaseWithEvaluation(() => Stop(context));
		}
		else
		{
			Stop(context);
		}
	}

	private async Task WaitUntil(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
		CancellationToken cancellationToken)
	{
		using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		Task timeoutOrCancellation = Task.Delay(timeout.ToTimerTimeout(), cts.Token);
		try
		{
			while (true)
			{
				Task recorded = GetRecordedSignal().Task;
				if (areFound(this) || await Task.WhenAny(recorded, timeoutOrCancellation) == timeoutOrCancellation)
				{
					return;
				}
			}
		}
		finally
		{
			// Releases the timer of the delay, which would otherwise run until the timeout.
			cts.Cancel();
		}
	}

	/// <remarks>
	///     The waiters continue asynchronously, so that they never run on the thread that raised the event.
	/// </remarks>
	private TaskCompletionSource<bool> GetRecordedSignal()
	{
		TaskCompletionSource<bool>? recorded = Volatile.Read(ref _recorded);
		if (recorded is not null)
		{
			return recorded;
		}

		TaskCompletionSource<bool> created = new(TaskCreationOptions.RunContinuationsAsynchronously);
		return Interlocked.CompareExchange(ref _recorded, created, null) ?? created;
	}

	private void NotifyRecordedEvent()
		=> Interlocked.Exchange(ref _recorded, null)?.TrySetResult(true);

	/// <summary>
	///     Keeps recording until <see cref="Dispose" /> instead of stopping with the next evaluation.
	/// </summary>
	public EventRecording<TSubject> UntilDisposed()
	{
		if (_isStopped)
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					"The recording was already stopped and cannot be continued. Call .UntilDisposed() before the first expectation."));
		}

		_stopsAfterEvaluation = false;
		return this;
	}

	private void Stop(IEvaluationContext? context)
	{
		_isStopped = true;
		_stoppedBy = context;
		foreach (EventRecorder recorder in _recorders)
		{
			recorder.Dispose();
		}
	}

	/// <remarks>
	///     A stopped recording is detached from the subject and answers from its frozen events, so a further expectation
	///     would silently check stale data and has to fail loudly instead. A recording that was stopped right away with a
	///     context that does not end with the evaluation can still be checked with that same context.
	/// </remarks>
	private void ThrowIfStopped(IEvaluationContext? context)
	{
		if (!_isStopped || (_stoppedBy is not null && ReferenceEquals(_stoppedBy, context)))
		{
			return;
		}

		throw Tracing.WriteException(
			new InvalidOperationException(_stopsAfterEvaluation
				? "The recording was already stopped. Use .UntilDisposed() to keep recording across multiple expectations."
				: "The recording was already disposed."));
	}

	/// <remarks>
	///     A recording of all events silently records nothing when reflection finds none, which under trimming means
	///     that they were removed, so the expectation that asks for the event has to fail loudly instead.
	/// </remarks>
	private int GetRecorderIndex(string eventName)
	{
		for (int i = 0; i < _recorders.Count; i++)
		{
			if (_recorders[i].Name == eventName)
			{
				return i;
			}
		}

		if (_skipped?.TryGetValue(eventName, out string? unsupported) == true)
		{
			throw Tracing.WriteException(new NotSupportedException(unsupported));
		}

		string nothingRecorded = _skipped is not null
			? "because no event was recorded"
			: "because no event was found";
		string recorded = _recorders.Count > 0
			? $"only {Formatter.Format(_recorders.Select(x => x.Name))}"
			: nothingRecorded;
		string skipped = _skipped is not null
			? $". No handler could be attached to {Formatter.Format(_skipped.Keys)}"
			: "";
		throw Tracing.WriteException(
			new NotSupportedException(
				$"Event {eventName} was not recorded on {_subjectExpression}, {recorded}{skipped}{(_isRegistered ? "." : TrimmingHint)}"));
	}

	/// <summary>
	///     The events recorded when <see cref="StopWhen" /> returned, so that the result of a constraint matches what it
	///     decided on, also while the recording continues for the next constraint.
	/// </summary>
	/// <remarks>
	///     A recorder only appends events, so the number of events of each recorder is enough to tell them apart from
	///     those recorded later.
	/// </remarks>
	private sealed class Snapshot : IEventRecordingResult
	{
		private readonly int[] _counts;
		private readonly EventRecording<TSubject> _recording;

		public Snapshot(EventRecording<TSubject> recording)
		{
			_recording = recording;
			_counts = new int[recording._recorders.Count];
			for (int i = 0; i < _counts.Length; i++)
			{
				_counts[i] = recording._recorders[i].Count;
			}
		}

		public int GetEventCount(string eventName, Func<object?[], bool>? filter = null)
		{
			int index = _recording.GetRecorderIndex(eventName);
			return _recording._recorders[index].GetEventCount(filter, _counts[index]);
		}

		public string ToString(string eventName)
		{
			int index = _recording.GetRecorderIndex(eventName);
			return _recording._recorders[index].ToString(_counts[index]);
		}

		public override string ToString()
			=> _recording.ToString();
	}
}
