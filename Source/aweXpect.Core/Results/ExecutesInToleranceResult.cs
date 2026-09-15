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
	///     …within the given <paramref name="tolerance" />.
	/// </summary>
	public TResult Within(TimeSpan tolerance)
	{
		options.Approximately(expected, tolerance);
		return returnValue;
	}
}
