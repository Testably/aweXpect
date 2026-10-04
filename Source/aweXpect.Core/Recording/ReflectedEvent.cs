using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using aweXpect.Core;

namespace aweXpect.Recording;

/// <summary>
///     An event that is bound reflectively, with the method of the <see cref="EventRecorder" /> that records it, or
///     the reason why it cannot be recorded.
/// </summary>
internal sealed class ReflectedEvent : IRecordableEvent
{
	/// <remarks>
	///     The events of a type and their handler methods never change, but are expensive to look up, so they are kept
	///     per type.
	/// </remarks>
	private static readonly ConcurrentDictionary<Type, ReflectedEvent[]> EventsByType = new();

	private ReflectedEvent(EventInfo info, MethodInfo? recordMethod, string? unsupported)
	{
		Info = info;
		RecordMethod = recordMethod;
		Unsupported = unsupported;
	}

	public EventInfo Info { get; }
	public MethodInfo? RecordMethod { get; }
	public string? Unsupported { get; }
	public string Name => Info.Name;

	public static ReflectedEvent[] GetAll(Type type)
		=> EventsByType.GetOrAdd(type, static key
			=> ReflectionFallback.IsSupported
				? key.GetEvents().Select(Create).ToArray()
				: throw Tracing.WriteException(ReflectionFallback.NotSupported(key, "events")));

	private static ReflectedEvent Create(EventInfo eventInfo)
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
			return new ReflectedEvent(eventInfo, null,
				$"The {eventInfo.Name} event cannot be recorded, because its handler returns {Formatter.Format(handlerType.ReturnType)}.");
		}

		ParameterInfo[] parameters = handlerType.GetParameters();
		ParameterInfo? byReference = parameters.FirstOrDefault(x => x.ParameterType.IsByRef);
		if (byReference is not null)
		{
			return new ReflectedEvent(eventInfo, null,
				$"The {eventInfo.Name} event cannot be recorded, because its handler takes the parameter {byReference.Name} by reference.");
		}

		MethodInfo? recordMethod = typeof(EventRecorder).GetMethods()
			.FirstOrDefault(x => x.Name == nameof(EventRecorder.RecordEvent) &&
			                     x.GetParameters().Length == parameters.Length);
		if (recordMethod is null)
		{
			return new ReflectedEvent(eventInfo, null,
				$"The {eventInfo.Name} event contains too many parameters ({parameters.Length}): {Formatter.Format(parameters.Select(x => x.ParameterType))}");
		}

		if (parameters.Length > 0)
		{
			recordMethod = recordMethod.MakeGenericMethod(parameters.Select(x => x.ParameterType).ToArray());
		}

		return new ReflectedEvent(eventInfo, recordMethod, null);
	}
}
