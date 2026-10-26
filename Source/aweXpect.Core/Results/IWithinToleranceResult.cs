using System;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TResult" /> directly after <c>Within(…)</c>, whose
///     <typeparamref name="TTolerance" /> can still be made relative to the expected value, e.g. via
///     <c>Percent()</c>.
/// </summary>
public interface IWithinToleranceResult<TResult, TTolerance>
{
	/// <summary>
	///     Interprets the specified tolerance relative to the expected value and returns the result to continue with.
	/// </summary>
	/// <param name="absoluteTolerance">
	///     Calculates the absolute tolerance from the specified tolerance and the expected value.
	/// </param>
	/// <param name="unit">The unit appended to the tolerance in the expectation text, e.g. <c>%</c>.</param>
	TResult SetRelativeTolerance(Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit);
}
