#if NET8_0_OR_GREATER
using System;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an allocation expectation on a synchronous delegate.
/// </summary>
public class AllocatesResult<TResult>
{
	private readonly AllocationOptions _options;
	private readonly TResult _returnValue;

	internal AllocatesResult(TResult returnValue, AllocationOptions options)
	{
		_returnValue = returnValue;
		_options = options;
	}

	/// <summary>
	///     …invoking the delegate once before the measured invocation, so that one-time allocations, e.g. of static
	///     constructors, lazy caches or pools, are not measured…
	/// </summary>
	/// <remarks>
	///     The delegate then runs twice. An exception in the warm-up fails the expectation like one in the measured
	///     invocation, which is then skipped, unless <c>AllowingExceptions()</c> is specified.
	/// </remarks>
	/// <exception cref="InvalidOperationException">The warm-up is already specified.</exception>
	public AllocatesResult<TResult> AfterWarmUp()
	{
		_options.AfterWarmUp();
		return this;
	}

	/// <summary>
	///     …allowing the delegate to throw an exception, measuring the allocations until it did so,
	///     according to the <paramref name="allowExceptions" /> parameter…
	/// </summary>
	/// <remarks>
	///     The measured allocations then include the exception and the stack trace the runtime captured for it.
	///     <para />
	///     A cancellation still fails the expectation, because it aborts the execution instead of measuring it.
	/// </remarks>
	/// <exception cref="InvalidOperationException">The exceptions are already specified.</exception>
	public AllocatesResult<TResult> AllowingExceptions(bool allowExceptions = true)
	{
		_options.AllowingExceptions(allowExceptions);
		return this;
	}

	/// <summary>
	///     …at least <paramref name="minimum" /> bytes.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="minimum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public TResult AtLeast(long minimum)
	{
		_options.AtLeast(minimum);
		return _returnValue;
	}

	/// <summary>
	///     …at most <paramref name="maximum" /> bytes.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public TResult AtMost(long maximum)
	{
		_options.AtMost(maximum);
		return _returnValue;
	}

	/// <summary>
	///     …between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TResult, long> Between(long minimum) => new(maximum =>
	{
		_options.Between(minimum, maximum);
		return _returnValue;
	});

	/// <summary>
	///     …nothing.
	/// </summary>
	/// <remarks>
	///     The shorthand for <c>AtMost(0)</c>.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public TResult Nothing()
	{
		_options.Nothing();
		return _returnValue;
	}
}
#endif
