using System;
using System.Threading;

namespace aweXpect.Customization;

/// <summary>
///     The behaviour for canceling tests.
/// </summary>
public class TestCancellation
{
	internal readonly Func<CancellationToken>? CancellationTokenFactory;
	internal readonly TimeSpan? Timeout;

	private TestCancellation(TimeSpan? timeout, Func<CancellationToken>? cancellationTokenFactory)
	{
		Timeout = timeout;
		CancellationTokenFactory = cancellationTokenFactory;
	}

	/// <summary>
	///     Applies the <paramref name="timeout" /> to all tests.
	/// </summary>
	public static TestCancellation FromTimeout(TimeSpan timeout)
		=> new(timeout, null);

	/// <summary>
	///     Uses a <see cref="CancellationToken" /> from the <paramref name="cancellationTokenFactory" /> in all tests.
	/// </summary>
	public static TestCancellation FromCancellationToken(Func<CancellationToken> cancellationTokenFactory)
		=> new(null, cancellationTokenFactory);

	/// <summary>
	///     Do not apply any cancellation of tests.
	/// </summary>
	public static TestCancellation None()
		=> new(null, null);
}
