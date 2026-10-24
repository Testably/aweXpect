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

	/// <remarks>
	///     Async-local, as an expectation is evaluated and its failure message is built across awaits, and its formatters
	///     must not reach other expectations, e.g. of tests that run in parallel.
	/// </remarks>
	private static readonly AsyncLocal<IValueFormatter[]?> ScopedFormattersInFlow = new();

	/// <summary>
	///     The default string representation of <see langword="null" />.
	/// </summary>
	public static readonly string NullString = "<null>";

#pragma warning disable S1118 // Utility classes should not have public constructors
	internal ValueFormatter() { }
#pragma warning restore S1118

	internal static Registration[] Registrations => Volatile.Read(ref _registrations);

	/// <summary>
	///     The formatters of the expectations that are currently evaluated or formatted in this async flow, or
	///     <see langword="null" /> outside of them. The most recent one is last.
	/// </summary>
	internal static IValueFormatter[]? ScopedFormatters => ScopedFormattersInFlow.Value;

	/// <summary>
	///     Adds the <paramref name="formatters" /> to the <see cref="ScopedFormatters" /> until the returned scope is
	///     disposed.
	/// </summary>
	/// <remarks>
	///     Without <paramref name="formatters" />, nothing changes.
	/// </remarks>
	internal static FormatterScope UseScopedFormatters(IValueFormatter[]? formatters)
	{
		if (formatters is null)
		{
			return default;
		}

		IValueFormatter[]? previous = ScopedFormattersInFlow.Value;
		ScopedFormattersInFlow.Value = previous is null ? formatters : [..previous, ..formatters,];
		return new FormatterScope(previous);
	}

	/// <summary>
	///     Registers a custom <paramref name="formatter" /> to use for formatting values.
	/// </summary>
	/// <remarks>
	///     The registration is process-wide: it applies to all threads and async flows, including tests that run in
	///     parallel, until the returned <see cref="IDisposable" /> is disposed.
	///     <para />
	///     It is consulted for every value that is not <see langword="null" />, whichever overload of
	///     <see cref="ValueFormatters" /> formats it. It may format nested values through them, and a value that is
	///     reached again while it is being formatted, including the value it accepted, is written as a recursion.
	///     <para />
	///     When several registered formatters can format a value, the most recently registered one is used. The
	///     formatters of an expectation, see <see cref="Core.ExpectationBuilder.AddFormatter" />, are consulted first.
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

	/// <summary>
	///     Restores the previous <see cref="ScopedFormatters" /> when it is disposed, see
	///     <see cref="UseScopedFormatters" />.
	/// </summary>
	internal readonly struct FormatterScope : IDisposable
	{
		private readonly IValueFormatter[]? _previous;
		private readonly bool _isActive;

		public FormatterScope(IValueFormatter[]? previous)
		{
			_previous = previous;
			_isActive = true;
		}

		public void Dispose()
		{
			if (_isActive)
			{
				ScopedFormattersInFlow.Value = _previous;
			}
		}
	}
}
