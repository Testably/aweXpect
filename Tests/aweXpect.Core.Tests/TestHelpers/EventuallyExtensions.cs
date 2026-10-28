using aweXpect.Delegates;

namespace aweXpect.Core.Tests.TestHelpers;

internal static class EventuallyExtensions
{
	/// <summary>
	///     Lets the attempts measure the time and wait on the virtual clock of the <paramref name="timeProvider" />, or
	///     of a new one.
	/// </summary>
	/// <remarks>
	///     See <see cref="VirtualTimeProvider" /> for what moves its clock.
	/// </remarks>
	public static EventuallySubject<T> OnVirtualTime<T>(this EventuallySubject<T> subject,
		VirtualTimeProvider? timeProvider = null)
	{
		((IExpectThat<T>)subject).ExpectationBuilder.UseTimeProvider(timeProvider ?? new VirtualTimeProvider());
		return subject;
	}
}
