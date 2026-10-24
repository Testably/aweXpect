using System;

namespace aweXpect.Results;

/// <summary>
///     An intermediate type to collect the maximum of the range.
/// </summary>
public class BetweenResult<TResult, TValue>(
	Func<TValue, TResult> callback)
{
	/// <summary>
	///     …and the <paramref name="maximum" /> value.
	/// </summary>
	public TResult And(TValue maximum)
		=> callback(maximum);
}
