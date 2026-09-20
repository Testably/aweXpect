using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an execution time expectation with no underlying value.
/// </summary>
public class ExecutesInResult<TResult>(
	TResult returnValue,
	ExecutionTimeOptions options)
	: IOptionsProvider<ExecutionTimeOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ExecutionTimeOptions IOptionsProvider<ExecutionTimeOptions>.Options => options;

	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the duration until it did so…
	/// </summary>
	/// <remarks>
	///     A cancellation still fails the expectation, because it aborts the execution instead of timing it.
	/// </remarks>
	public ExecutesInResult<TResult> AllowingExceptions()
	{
		options.AllowExceptions();
		return this;
	}

	/// <summary>
	///     …at most <paramref name="maximum" /> time.
	/// </summary>
	public TResult AtMost(TimeSpan maximum)
	{
		options.AtMost(maximum);
		return returnValue;
	}

	/// <summary>
	///     …at least <paramref name="minimum" /> time.
	/// </summary>
	public TResult AtLeast(TimeSpan minimum)
	{
		options.AtLeast(minimum);
		return returnValue;
	}

	/// <summary>
	///     …between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult Between(TimeSpan minimum) => new(maximum =>
	{
		options.Between(minimum, maximum);
		return returnValue;
	});

	/// <summary>
	///     An intermediate type to collect the maximum of the time range.
	/// </summary>
	public class BetweenResult(
		Func<TimeSpan, TResult> callback)
	{
		/// <summary>
		///     …and <paramref name="maximum" /> time.
		/// </summary>
		public TResult And(TimeSpan maximum)
			=> callback(maximum);
	}
}
