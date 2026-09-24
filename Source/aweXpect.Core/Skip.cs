using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Initialization;

namespace aweXpect;

/// <summary>
///     Methods for dynamically skipping the running test.
/// </summary>
[StackTraceHidden]
public static class Skip
{
	/// <summary>
	///     Dynamically skips the current test.
	///     <para />
	///     This is used when the decision whether a test should be skipped happens at runtime rather than at discovery time.
	/// </summary>
	/// <param name="reason">The reason why the test was skipped.</param>
	[DoesNotReturn]
	public static void Test(string reason)
		=> SkipIf(true, reason);

	/// <summary>
	///     Dynamically skips the current test when the <paramref name="condition" /> is <see langword="false" />.
	/// </summary>
	/// <param name="condition">When <see langword="false" />, the test will be skipped; otherwise it will continue to run.</param>
	/// <param name="reason">The reason why the test was skipped.</param>
	public static void Unless([DoesNotReturnIf(false)] bool condition, string reason)
		=> SkipIf(!condition, reason);

	/// <summary>
	///     Dynamically skips the current test when the <paramref name="condition" /> is <see langword="true" />.
	/// </summary>
	/// <param name="condition">When <see langword="true" />, the test will be skipped; otherwise it will continue to run.</param>
	/// <param name="reason">The reason why the test was skipped.</param>
	public static void When([DoesNotReturnIf(true)] bool condition, string reason)
		=> SkipIf(condition, reason);

	private static void SkipIf([DoesNotReturnIf(true)] bool condition, string reason)
	{
		if (!condition)
		{
			return;
		}

		AweXpectInitialization.State.Value.Skip(reason);
	}
}
