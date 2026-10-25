using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation that still requires a tolerance.
/// </summary>
public class ExecutesInToleranceResult<TResult>(
	TResult returnValue,
	OptionsSlot<ExecutionTimeOptions> options,
	TimeSpan expected)
{
	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInToleranceResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		options.Value.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …within the given <paramref name="tolerance" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult Within(TimeSpan tolerance)
	{
		options.Value.Approximately(expected, tolerance);
		return returnValue;
	}
}
