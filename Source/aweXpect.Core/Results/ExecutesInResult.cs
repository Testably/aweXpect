using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation with no underlying value.
/// </summary>
public class ExecutesInResult<TResult>(
	TResult returnValue,
	OptionsSlot<ExecutionTimeOptions> options)
	: IOptionsProvider<ExecutionTimeOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<ExecutionTimeOptions> IOptionsProvider<ExecutionTimeOptions>.Options => options;

	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		options.Value.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …at most <paramref name="maximum" /> time.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult AtMost(TimeSpan maximum)
	{
		options.AtMost(maximum);
		return returnValue;
	}

	/// <summary>
	///     …at least <paramref name="minimum" /> time.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult AtLeast(TimeSpan minimum)
	{
		options.AtLeast(minimum);
		return returnValue;
	}

	/// <summary>
	///     …between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TResult, TimeSpan> Between(TimeSpan minimum) => new(maximum =>
	{
		options.Between(minimum, maximum);
		return returnValue;
	});
}
