using aweXpect.Delegates;

namespace aweXpect.Core.Tests.TestHelpers;

internal static class EventuallyExtensions
{
	/// <summary>
	///     Lets the attempts measure the time and wait on the virtual clock of the <paramref name="timeProvider" />, or
	///     of a new <see cref="MockTimeSystem" />.
	/// </summary>
	public static EventuallySubject<T> OnVirtualTime<T>(this EventuallySubject<T> subject,
		TimeProvider? timeProvider = null)
	{
		((IExpectThat<T>)subject).ExpectationBuilder.UseTimeProvider(timeProvider ?? new MockTimeSystem().ToTimeProvider());
		return subject;
	}
}
