using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation that still requires a tolerance.
/// </summary>
/// <remarks>
///     The <paramref name="expected" /> time plus the tolerance also limits the evaluation of the
///     <paramref name="expectationBuilder" />: the result adds itself as timeout bound, which reads the options when the
///     evaluation starts, so that a later tolerance replaces the bound of an earlier one.
/// </remarks>
public class ExecutesInToleranceResult<TResult>(
	ExpectationBuilder expectationBuilder,
	TResult returnValue,
	IOptionsHolderSource<ExecutionTimeOptions> options,
	TimeSpan expected) : ITimeoutBound
{
	/// <inheritdoc cref="ITimeoutBound.UpperBound" />
	TimeSpan? ITimeoutBound.UpperBound => options.Holder.Value.UpperBound;

	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInToleranceResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		options.Holder.Value = options.Holder.Value.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …within the given <paramref name="tolerance" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult Within(TimeSpan tolerance)
	{
		options.Holder.Value = options.Holder.Value.Approximately(expected, tolerance);
		expectationBuilder.AddTimeoutBound(this);
		return returnValue;
	}
}
