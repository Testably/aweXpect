using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;
#if NET8_0_OR_GREATER
using System.Threading.Channels;
#endif

namespace aweXpect.Recording;

internal sealed class EventRecording<TSubject> : IEventRecording<TSubject>, IEventRecordingResult
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
	private readonly Dictionary<string, EventRecorder> _recorders = new();
	private readonly string _subjectExpression;

	/// <summary>
	///     Creates a new recording the given <paramref name="eventNames" /> that are triggered on the
	///     <paramref name="subject" />.
	/// </summary>
	public EventRecording(TSubject subject, string subjectExpression, params string[] eventNames)
	{
		_subjectExpression = subjectExpression;
		_isRegistered = TryGetRegistered(subject.GetType(), out List<RecordableEvent> events);
		if (!_isRegistered)
		{
			events = Reflect(subject.GetType());
		}

		if (eventNames.Length == 0)
		{
			eventNames = events.Select(x => x.Name).ToArray();
		}

		foreach (string? eventName in eventNames)
		{
			EventRecorder recorder = new(eventName);
			_recorders.Add(eventName, recorder);
			RecordableEvent? @event = events.FirstOrDefault(x => x.Name == eventName);
			if (@event == null)
			{
				throw new NotSupportedException(
						$"Event {eventName} is not supported on {Formatter.Format(subject)}{(_isRegistered ? "" : TrimmingHint)}")
					.LogTrace();
			}

			@event.Attach(recorder, new WeakReference(subject));
		}
	}

	/// <remarks>
	///     A type counts as registered only when it has an event: a registration of its members says nothing about the
	///     events, so such a type is reflected over like an unregistered one.
	/// </remarks>
	private static bool TryGetRegistered(Type type, out List<RecordableEvent> events)
	{
		if (TypeMetadataRegistry.Instance.TryGet(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    !metadata.Events.IsEmpty)
		{
			events = metadata.Events.Values
				.OrderBy(x => x.Order)
				.Select(x => new RecordableEvent(x.Name, (recorder, subject) => recorder.Attach(subject, x)))
				.ToList();
			return true;
		}

		events = [];
		return false;
	}

	private static List<RecordableEvent> Reflect(Type type)
		=> type.GetEvents()
			.Select(x => new RecordableEvent(x.Name, (recorder, subject) => recorder.Attach(subject, x)))
			.ToList();

	private sealed class RecordableEvent(string name, Action<EventRecorder, WeakReference> attach)
	{
		public string Name { get; } = name;

		public void Attach(EventRecorder recorder, WeakReference subject) => attach(recorder, subject);
	}

#if NET8_0_OR_GREATER
	public async Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout)
	{
		if (timeout > TimeSpan.Zero && !areFound(this))
		{
			Channel<bool> channel = Channel.CreateUnbounded<bool>();
			using CancellationTokenSource cts = new(timeout);
			CancellationToken token = cts.Token;
			foreach (EventRecorder recorder in _recorders.Values)
			{
				recorder.Register(channel.Writer);
			}

			try
			{
#pragma warning disable S3267 // https://rules.sonarsource.com/csharp/RSPEC-3267
				await foreach (bool _ in channel.Reader.ReadAllAsync(token))
				{
					if (areFound(this))
					{
						break;
					}
				}
#pragma warning restore S3267
			}
			catch (OperationCanceledException)
			{
				// Ignore cancellation
			}
		}

		foreach (EventRecorder recorder in _recorders.Values)
		{
			recorder.Dispose();
		}

		return this;
	}
#else
	public Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout)
	{
		DateTime now = DateTime.Now;
		DateTime endTime = now.Add(timeout);
		if (timeout > TimeSpan.Zero && !areFound(this))
		{
			using (ManualResetEventSlim ms = new())
			{
				foreach (EventRecorder recorder in _recorders.Values)
				{
					recorder.Register(ms);
				}

				while (true)
				{
					now = DateTime.Now;
					if (now >= endTime)
					{
						break;
					}

					ms.Reset();
					ms.Wait(endTime - now);
					if (areFound(this))
					{
						break;
					}
				}
			}
		}

		foreach (EventRecorder recorder in _recorders.Values)
		{
			recorder.Dispose();
		}

		return Task.FromResult<IEventRecordingResult>(this);
	}
#endif

	/// <summary>
	///     Gets the number of recorded events for <paramref name="eventName" /> that match the <paramref name="filter" />.
	/// </summary>
	public int GetEventCount(string eventName, Func<object?[], bool>? filter = null)
		=> GetRecorder(eventName).GetEventCount(filter);

	/// <summary>
	///     Returns a formatted string for the recorded events for <paramref name="eventName" />.
	/// </summary>
	public string ToString(string eventName)
		=> GetRecorder(eventName).ToString();

	/// <remarks>
	///     A recording of all events silently records nothing when reflection finds none, which under trimming means
	///     that they were removed, so the expectation that asks for the event has to fail loudly instead.
	/// </remarks>
	private EventRecorder GetRecorder(string eventName)
	{
		if (!_recorders.TryGetValue(eventName, out EventRecorder? recorder))
		{
			throw new NotSupportedException(
					$"Event {eventName} was not recorded on {_subjectExpression}, only {Formatter.Format(_recorders.Keys)}{(_isRegistered ? "" : TrimmingHint)}")
				.LogTrace();
		}

		return recorder;
	}

	/// <inheritdoc />
	public override string ToString()
		=> _subjectExpression;
}
