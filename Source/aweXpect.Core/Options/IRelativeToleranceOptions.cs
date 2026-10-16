using System;

namespace aweXpect.Options;

/// <summary>
///     Options whose <typeparamref name="TTolerance" /> can be made relative to the expected value.
/// </summary>
public interface IRelativeToleranceOptions<TTolerance>
{
	/// <summary>
	///     Interprets the specified tolerance relative to the expected value.
	/// </summary>
	/// <param name="absoluteTolerance">
	///     Calculates the absolute tolerance from the specified tolerance and the expected value.
	/// </param>
	/// <param name="unit">The unit appended to the tolerance in the expectation text, e.g. <c>%</c>.</param>
	void UseRelativeTolerance(Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit);
}
