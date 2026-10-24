using System;

namespace aweXpect.Results;

/// <summary>
///     An intermediate type to collect the number of times.
/// </summary>
public class CountTimesResult<TResult>(
	Func<int, TResult> callback)
{
	/// <summary>
	///     …once.
	/// </summary>
	public TResult Once()
		=> callback(1);

	/// <summary>
	///     …twice.
	/// </summary>
	public TResult Twice()
		=> callback(2);
}
