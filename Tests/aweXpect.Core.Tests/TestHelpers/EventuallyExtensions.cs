using aweXpect.Delegates;

namespace aweXpect.Core.Tests.TestHelpers;

internal static class EventuallyExtensions
{
	/// <summary>
	///     Lets the attempts measure the time and wait on the virtual clock of the <paramref name="timeSystem" />, or
	///     of a new one.
	/// </summary>
	/// <remarks>
	///     See <see cref="VirtualTimeSystem" /> for the subjects that still depend on real time.
	/// </remarks>
	public static EventuallySubject<T> OnVirtualTime<T>(this EventuallySubject<T> subject,
		VirtualTimeSystem? timeSystem = null)
	{
		((IExpectThat<T>)subject).ExpectationBuilder.UseTimeSystem(timeSystem ?? new VirtualTimeSystem());
		return subject;
	}
}
