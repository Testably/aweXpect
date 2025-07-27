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
}
