using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Initialization;

namespace aweXpect;

/// <summary>
///     Methods for explicitly failing the running test.
/// </summary>
[StackTraceHidden]
public static class Fail
{
	/// <summary>
	///     Explicitly fails the current test.
	/// </summary>
	/// <param name="reason">The reason why the test failed</param>
	/// <param name="innerException">
	///     The optional exception that caused the failure. It is forwarded as inner exception of the
	///     framework-specific assertion exception, so that its original stack trace remains available.
	/// </param>
	[DoesNotReturn]
	public static void Test(string reason, Exception? innerException = null)
		=> FailIf(true, reason, innerException);

	/// <summary>
	///     Explicitly fails the current test when the <paramref name="condition" /> is <c>false</c>.
	/// </summary>
	/// <param name="condition">When <c>false</c>, the test will be failed; otherwise it will continue to run</param>
	/// <param name="reason">The reason why the test was failed</param>
	/// <param name="innerException">
	///     The optional exception that caused the failure. It is forwarded as inner exception of the
	///     framework-specific assertion exception, so that its original stack trace remains available.
	/// </param>
	public static void Unless([DoesNotReturnIf(false)] bool condition, string reason, Exception? innerException = null)
		=> FailIf(!condition, reason, innerException);

	/// <summary>
	///     Explicitly fails the current test when the <paramref name="condition" /> is <c>true</c>.
	/// </summary>
	/// <param name="condition">When <c>true</c>, the test will be failed; otherwise it will continue to run</param>
	/// <param name="reason">The reason why the test was failed</param>
	/// <param name="innerException">
	///     The optional exception that caused the failure. It is forwarded as inner exception of the
	///     framework-specific assertion exception, so that its original stack trace remains available.
	/// </param>
	public static void When([DoesNotReturnIf(true)] bool condition, string reason, Exception? innerException = null)
		=> FailIf(condition, reason, innerException);

	/// <summary>
	///     Explicitly fails the current test as inconclusive.
	/// </summary>
	/// <param name="reason">The reason why the test failed</param>
	[DoesNotReturn]
	public static void Inconclusive(string reason)
		=> AweXpectInitialization.State.Value.Inconclusive(reason);

	private static void FailIf([DoesNotReturnIf(true)] bool condition, string reason, Exception? innerException)
	{
		if (!condition)
		{
			return;
		}

		AweXpectInitialization.State.Value.Fail(reason, innerException);
	}
}
