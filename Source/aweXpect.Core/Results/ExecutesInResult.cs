using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation with no underlying value.
/// </summary>
/// <remarks>
///     An option with an upper bound also limits the evaluation of the <paramref name="expectationBuilder" />: the
///     result adds itself as timeout bound, which reads the options when the evaluation starts, so that a later option
///     replaces the bound of an earlier one.
/// </remarks>
public class ExecutesInResult<TResult>(
	ExpectationBuilder expectationBuilder,
	TResult returnValue,
	IOptionsHolderSource<ExecutionTimeOptions> options)
	: IOptionsProvider<ExecutionTimeOptions>, ITimeoutBound
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ExecutionTimeOptions> IOptionsProvider<ExecutionTimeOptions>.Options => options.Holder;

	/// <inheritdoc cref="ITimeoutBound.UpperBound" />
	TimeSpan? ITimeoutBound.UpperBound => options.Holder.Value.UpperBound;

	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		options.Holder.Value = options.Holder.Value.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …at most <paramref name="maximum" /> time.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult AtMost(TimeSpan maximum)
	{
		options.Holder.AtMost(maximum);
		expectationBuilder.AddTimeoutBound(this);
		return returnValue;
	}

	/// <summary>
	///     …at least <paramref name="minimum" /> time.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public TResult AtLeast(TimeSpan minimum)
	{
		options.Holder.AtLeast(minimum);
		return returnValue;
	}

	/// <summary>
	///     …between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TResult, TimeSpan> Between(TimeSpan minimum) => new(maximum =>
	{
		options.Holder.Between(minimum, maximum);
		expectationBuilder.AddTimeoutBound(this);
		return returnValue;
	});
}
