using System;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation that still requires a tolerance.
/// </summary>
public class ExecutesInToleranceResult<TResult>(
	TResult returnValue,
	TimeSpanEqualityOptions options,
	TimeSpan expected)
{
	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInToleranceResult<TResult> AllowingExceptions()
	{
		options.AllowExceptions();
		return this;
	}

	/// <summary>
	///     …within the given <paramref name="tolerance" />.
	/// </summary>
	public TResult Within(TimeSpan tolerance)
	{
		options.Approximately(expected, tolerance);
		return returnValue;
	}
}
