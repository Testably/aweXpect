using System;
using System.Linq;
using System.Threading;
using aweXpect.Customization;

namespace aweXpect.Formatting;

/// <summary>
///     Formatter for arbitrary objects in exception messages.
/// </summary>
public class ValueFormatter
{
	private static readonly object RegistrationLock = new();

	/// <remarks>
	///     Replaced instead of changed, so that formatting reads it without a lock. The most recent registration is
	///     last.
	/// </remarks>
	private static Registration[] _registrations = [];

	/// <summary>
	///     The default string representation of <see langword="null" />.
	/// </summary>
	public static readonly string NullString = "<null>";

#pragma warning disable S1118 // Utility classes should not have public constructors
	internal ValueFormatter() { }
#pragma warning restore S1118

	internal static Registration[] Registrations => Volatile.Read(ref _registrations);

	/// <summary>
	///     Registers a custom <paramref name="formatter" /> to use for formatting <see cref="object" />s.
	/// </summary>
	/// <remarks>
	///     The registration is process-wide: it applies to all threads and async flows, including tests that run in
	///     parallel, until the returned <see cref="IDisposable" /> is disposed.
	///     <para />
	///     When several registered formatters can format a value, the most recently registered one is used.
	/// </remarks>
	public static IDisposable Register(IValueFormatter formatter)
	{
		Registration registration = new(formatter);
		lock (RegistrationLock)
		{
			Volatile.Write(ref _registrations, [.._registrations, registration,]);
		}

		return new CustomizationLifetime(() =>
		{
			lock (RegistrationLock)
			{
				Volatile.Write(ref _registrations, _registrations.Where(x => x != registration).ToArray());
			}
		});
	}

	/// <remarks>
	///     Gives each registration its own identity, so that disposing it removes only this registration, even when
	///     the same formatter was registered more than once.
	/// </remarks>
	internal sealed class Registration(IValueFormatter formatter)
	{
		public IValueFormatter Formatter { get; } = formatter;
	}
}
