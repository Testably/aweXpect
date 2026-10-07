using System;
using System.Text;
using System.Threading;
using aweXpect.Core.Metadata;

namespace aweXpect.Recording;

internal sealed class EventRecorder(string eventName, Action onRecorded) : IDisposable
{
	private readonly object _lock = new();

	private int _count;

	private IRecordableEvent? _event;

	/// <remarks>
	///     Only appended to, and replaced by a larger copy before the count grows beyond it, so that a reader that reads
	///     the count before the array sees every event below the count without a lock, and a snapshot needs only the
	///     count.
	/// </remarks>
	private RecordedEvent[] _events = [];

	/// <remarks>
	///     Removing the handler cannot stop an invocation that already started, so a stopped recorder answers from the
	///     events it had when it stopped, and an event that arrives later is ignored.
	/// </remarks>
	private int _frozenCount = -1;

	private Delegate? _handler;
	private object? _subject;

	public string Name => eventName;

	/// <summary>
	///     The number of recorded events, which only grows until the recorder is stopped.
	/// </summary>
	public int Count
	{
		get
		{
			int frozenCount = Volatile.Read(ref _frozenCount);
			return frozenCount >= 0 ? frozenCount : Volatile.Read(ref _count);
		}
	}

	public void Dispose()
	{
		switch (_event)
		{
			case TypeMetadataRegistry.RegisteredEvent registeredEvent:
				registeredEvent.RemoveHandler(_subject!, _handler!);
				break;
			case ReflectedEvent reflectedEvent:
				reflectedEvent.Info.RemoveEventHandler(_subject, _handler);
				break;
		}

		Interlocked.CompareExchange(ref _frozenCount, Volatile.Read(ref _count), -1);
	}

	/// <summary>
	///     Attaches to the <paramref name="event" /> and returns the reason why it cannot be recorded, or
	///     <see langword="null" /> when the handler was attached.
	/// </summary>
	/// <remarks>
	///     The subject is held on purpose: its event already holds the handler and thereby this recorder, so nothing
	///     leaks, whereas a static event would otherwise keep the handler after the subject was collected.
	/// </remarks>
	public string? TryAttach(object subject, IRecordableEvent @event)
	{
		if (@event is ReflectedEvent reflectedEvent)
		{
			if (reflectedEvent.Unsupported is not null)
			{
				return reflectedEvent.Unsupported;
			}

			_handler = Delegate.CreateDelegate(reflectedEvent.Info.EventHandlerType!, this,
				reflectedEvent.RecordMethod!);
			reflectedEvent.Info.AddEventHandler(subject, _handler);
		}
		else
		{
			TypeMetadataRegistry.RegisteredEvent registeredEvent = (TypeMetadataRegistry.RegisteredEvent)@event;
			_handler = registeredEvent.CreateHandler(Record);
			registeredEvent.AddHandler(subject, _handler);
		}

		_subject = subject;
		_event = @event;
		return null;
	}

	public void RecordEvent()
		=> Record([]);

	public void RecordEvent<T1>(T1 parameter1)
		=> Record([parameter1,]);

	public void RecordEvent<T1, T2>(T1 parameter1, T2 parameter2)
		=> Record([parameter1, parameter2,]);

	public void RecordEvent<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
		=> Record([parameter1, parameter2, parameter3,]);

	public void RecordEvent<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
		=> Record([parameter1, parameter2, parameter3, parameter4,]);

	private void Record(object?[] parameters)
	{
		lock (_lock)
		{
			RecordedEvent[] events = _events;
			if (_count == events.Length)
			{
				RecordedEvent[] grown = new RecordedEvent[Math.Max(4, events.Length * 2)];
				Array.Copy(events, grown, _count);
				Volatile.Write(ref _events, grown);
				events = grown;
			}

			events[_count] = new RecordedEvent(eventName, parameters);
			Volatile.Write(ref _count, _count + 1);
		}

		onRecorded();
	}

	/// <summary>
	///     Returns a formatted string for the first <paramref name="count" /> recorded events.
	/// </summary>
	public string ToString(int count)
		=> Formatter.Format(new ArraySegment<RecordedEvent>(Volatile.Read(ref _events), 0, count),
			FormattingOptions.MultipleLines);

	/// <summary>
	///     Returns a formatted string for all recorded events.
	/// </summary>
	public override string ToString()
		=> ToString(Count);

	/// <summary>
	///     Gets the number of recorded events that match the <paramref name="filter" />.
	/// </summary>
	public int GetEventCount(Func<object?[], bool>? filter)
		=> GetEventCount(filter, Count);

	/// <summary>
	///     Gets the number of events among the first <paramref name="count" /> recorded events that match the
	///     <paramref name="filter" />.
	/// </summary>
	public int GetEventCount(Func<object?[], bool>? filter, int count)
	{
		if (filter is null)
		{
			return count;
		}

		RecordedEvent[] events = Volatile.Read(ref _events);
		int matchingCount = 0;
		for (int i = 0; i < count; i++)
		{
			if (filter(events[i].Parameters))
			{
				matchingCount++;
			}
		}

		return matchingCount;
	}

	private readonly struct RecordedEvent(string name, object?[] parameters)
	{
		public string Name { get; } = name;
		public object?[] Parameters { get; } = parameters;

		/// <inheritdoc />
		public override string ToString()
		{
			StringBuilder sb = new();
			sb.Append(Name).Append('(');
			if (Parameters.Length > 0)
			{
				foreach (object? parameter in Parameters)
				{
					Formatter.Format(sb, parameter);
					sb.Append(", ");
				}

				sb.Length -= 2;
			}

			sb.Append(')');
			return sb.ToString();
		}
	}
}
