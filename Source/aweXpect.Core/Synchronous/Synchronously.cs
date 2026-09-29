using System.Threading;
using aweXpect.Results;

namespace aweXpect.Synchronous;

/// <summary>
///     Methods to support synchronous execution.
/// </summary>
/// <remarks>
///     <b>WARNING!</b><br />
///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
/// </remarks>
public static class Synchronously
{
	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
	/// </remarks>
	public static void Verify(ExpectationResult result)
	{
		SynchronizationContext? context = SynchronizationContext.Current;
		if (context is null)
		{
			result.GetAwaiter().GetResult();
			return;
		}

		// A continuation posted to the context of the blocked thread would never run.
		SynchronizationContext.SetSynchronizationContext(null);
		try
		{
			result.GetAwaiter().GetResult();
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(context);
		}
	}

	/// <summary>
	///     Verifies synchronously that the expectation is satisfied.
	/// </summary>
	/// <remarks>
	///     <b>WARNING!</b><br />
	///     The only intended use case is to support synchronous evaluation for <c>ref struct</c>.
	/// </remarks>
	public static TType Verify<TType, TSelf>(ExpectationResult<TType, TSelf> result)
		where TSelf : ExpectationResult<TType, TSelf>
	{
		SynchronizationContext? context = SynchronizationContext.Current;
		if (context is null)
		{
			return result.GetAwaiter().GetResult();
		}

		SynchronizationContext.SetSynchronizationContext(null);
		try
		{
			return result.GetAwaiter().GetResult();
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(context);
		}
	}
}
