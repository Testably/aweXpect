using System.Reflection;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	private static EventInfo? ExpectedEventInfo()
		=> typeof(SomeClassToVerifyTheEventNameOfIt)
			.GetEvent(nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt));

	private class SomeClassToVerifyTheEventNameOfIt
	{
		public event EventHandler SomeEventToVerifyTheNameOfIt = null!;
	}
#pragma warning disable CS0067 // The event is never used
	private class SealedEventClass : AbstractEventClass
	{
		public sealed override event Action? AbstractEvent;
		public sealed override event Action? VirtualEvent;
	}

	private abstract class AbstractEventClass
	{
		public abstract event Action? AbstractEvent;
		public virtual event Action? VirtualEvent;
	}

	private class ConcreteEventClass
	{
		public virtual event Action? VirtualEvent;
		public event Action? ConcreteEvent;
	}
#pragma warning restore CS0067
}
