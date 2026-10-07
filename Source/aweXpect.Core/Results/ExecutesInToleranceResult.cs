using System;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation that still requires a tolerance.
/// </summary>
public class ExecutesInToleranceResult<TResult>(
	TResult returnValue,
	ExecutionTimeOptions options,
	TimeSpan expected)
{
	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	/// <exception cref="InvalidOperationException">The exceptions are already specified.</exception>
	public ExecutesInToleranceResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		options.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …within the given <paramref name="tolerance" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public TResult Within(TimeSpan tolerance)
	{
		options.Approximately(expected, tolerance);
		return returnValue;
	}
}
