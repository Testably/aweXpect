#if NET8_0_OR_GREATER
using System.Threading.Channels;
#else
using System.Threading;
#endif
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Metadata;

namespace aweXpect.Recording;

internal sealed class EventRecorder(string eventName) : IDisposable
{
	private readonly ConcurrentQueue<RecordedEvent> _eventQueue = new();
	private Action? _onDispose;
#if NET8_0_OR_GREATER
	private ChannelWriter<bool>? _channelWriter;
#else
	private SemaphoreSlim? _signal;
#endif

	public void Dispose()
	{
#if NET8_0_OR_GREATER
		_channelWriter = null;
#else
		_signal = null;
#endif
		_onDispose?.Invoke();
	}

	/// <summary>
	///     Attaches to a registered event, whose handler is created by the registration instead of being bound
	///     reflectively.
	/// </summary>
	public void Attach(object subject, TypeMetadataRegistry.RegisteredEvent @event)
	{
		Delegate handler = @event.CreateHandler(parameters =>
		{
			_eventQueue.Enqueue(new RecordedEvent(eventName, parameters));
			NotifyRecordedEvent();
		});
		@event.AddHandler(subject, handler);

		// The subject is held on purpose: its event already holds the handler and thereby this recorder, so nothing
		// leaks, whereas a static event would otherwise keep the handler after the subject was collected.
		_onDispose = () => @event.RemoveHandler(subject, handler);
	}

	/// <summary>
	///     Attaches to an event that is bound reflectively and returns the reason why it cannot be recorded, or
	///     <see langword="null" /> when the handler was attached.
	/// </summary>
	public string? TryAttach(object subject, EventInfo eventInfo)
	{
		// Unreachable, because the events are only ever found by the guarded reflection, but the analyzer does not
		// follow guards across methods.
		if (!ReflectionFallback.IsSupported)
		{
			throw Tracing.WriteException(ReflectionFallback.NotSupported(eventInfo.ReflectedType!, "events"));
		}

		MethodInfo handlerType = eventInfo.EventHandlerType!.GetMethod("Invoke")!;
		if (handlerType.ReturnType != typeof(void))
		{
			return
				$"The {eventName} event cannot be recorded, because its handler returns {Formatter.Format(handlerType.ReturnType)}";
		}

		ParameterInfo? byReference = handlerType.GetParameters().FirstOrDefault(x => x.ParameterType.IsByRef);
		if (byReference is not null)
		{
			return
				$"The {eventName} event cannot be recorded, because its handler takes the parameter {byReference.Name} by reference";
		}

		Delegate? handler = null;
		foreach (MethodInfo method in typeof(EventRecorder).GetMethods().Where(x => x.Name == nameof(RecordEvent)))
		{
			if (method.GetParameters().Length == handlerType.GetParameters().Length)
			{
				MethodInfo handlerMethod = method;
				if (handlerType.GetParameters().Length > 0)
				{
					handlerMethod = method
						.MakeGenericMethod(handlerType.GetParameters()
							.Select(x => x.ParameterType)
							.ToArray());
				}

				handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handlerMethod);
			}
		}

		if (handler == null)
		{
			return
				$"The {eventName} event contains too many parameters ({handlerType.GetParameters().Length}): {Formatter.Format(handlerType.GetParameters().Select(x => x.ParameterType))}";
		}

		eventInfo.AddEventHandler(subject, handler);

		// The subject is held on purpose: its event already holds the handler and thereby this recorder, so nothing
		// leaks, whereas a static event would otherwise keep the handler after the subject was collected.
		_onDispose = () => eventInfo.RemoveEventHandler(subject, handler);
		return null;
	}

	public void RecordEvent()
	{
		_eventQueue.Enqueue(new RecordedEvent(eventName));
		NotifyRecordedEvent();
	}

	public void RecordEvent<T1>(T1 parameter1)
	{
		_eventQueue.Enqueue(new RecordedEvent(eventName, parameter1));
		NotifyRecordedEvent();
	}

	public void RecordEvent<T1, T2>(T1 parameter1, T2 parameter2)
	{
		_eventQueue.Enqueue(new RecordedEvent(eventName, parameter1, parameter2));
		NotifyRecordedEvent();
	}

	public void RecordEvent<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
	{
		_eventQueue.Enqueue(new RecordedEvent(eventName, parameter1, parameter2, parameter3));
		NotifyRecordedEvent();
	}

	public void RecordEvent<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
	{
		_eventQueue.Enqueue(new RecordedEvent(eventName, parameter1, parameter2, parameter3, parameter4));
		NotifyRecordedEvent();
	}

#if NET8_0_OR_GREATER
	public void Register(ChannelWriter<bool>? channel)
		=> _channelWriter = channel;

	private void NotifyRecordedEvent() => _channelWriter?.TryWrite(true);
#else
	public void Register(SemaphoreSlim? signal)
		=> _signal = signal;

	private void NotifyRecordedEvent() => _signal?.Release();
#endif

	/// <summary>
	///     Returns a formatted string for all recorded events.
	/// </summary>
	public override string ToString()
		=> Formatter.Format(_eventQueue, FormattingOptions.MultipleLines);

	/// <summary>
	///     Gets the number of recorded events that match the <paramref name="filter" />.
	/// </summary>
	public int GetEventCount(Func<object?[], bool>? filter)
	{
		if (filter != null)
		{
			return _eventQueue.Count(x => filter(x.Parameters));
		}

		return _eventQueue.Count;
	}

	private readonly struct RecordedEvent(string name, params object?[] parameters)
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
