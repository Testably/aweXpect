using System;

namespace aweXpect.Core.Sources;

/// <summary>
///     An expectation source from a delegate can contain a <see cref="Value" /> or a thrown <see cref="Exception" />.
/// </summary>
public class DelegateValue<TValue>(in TValue? value, Exception? exception, TimeSpan duration, bool isNull = false)
	: DelegateValue(exception, duration, isNull)
{
	/// <summary>
	///     The value of the delegate, if no exception was thrown.
	/// </summary>
	public TValue? Value { get; } = value;

	internal override DelegateValue WithExceededTimeout(TimeSpan timeout, Exception exception)
		=> new DelegateValue<TValue>(default, exception, Duration, IsNull)
		{
			ExceededTimeout = timeout,
		};

	/// <inheritdoc />
	public override string ToString()
	{
		if (Exception == null)
		{
			return
				$"delegate returning {Formatter.Format(typeof(TValue))} {Formatter.Format(Value)} in {Formatter.Format(Duration)}";
		}

		return
			$"delegate returning {Formatter.Format(typeof(TValue))} throwing {Formatter.Format(Exception)} after {Formatter.Format(Duration)}";
	}
}

/// <summary>
///     An expectation source from a delegate without value can represent <see langword="void" /> or a thrown
///     <see cref="Exception" />.
/// </summary>
public class DelegateValue(Exception? exception, TimeSpan duration, bool isNull = false)
{
	/// <summary>
	///     The duration it took the delegate to complete.
	/// </summary>
	public TimeSpan Duration { get; } = duration;

	/// <summary>
	///     The thrown exception of the delegate.
	/// </summary>
	public Exception? Exception { get; } = exception;

	/// <summary>
	///     Flag, indicating if the delegate callback was <see langword="null" />.
	/// </summary>
	public bool IsNull { get; } = isNull;

	/// <summary>
	///     The timeout within which the delegate did not finish, so that the evaluation stopped waiting for it, or
	///     <see langword="null" /> if it finished in time or no timeout applied.
	/// </summary>
	/// <remarks>
	///     The <see cref="Exception" /> is then a <see cref="TimeoutException" />, whichever way the delegate reacted to
	///     the cancellation, so that a timeout is reported the same way every time.
	/// </remarks>
	public TimeSpan? ExceededTimeout { get; private protected set; }

	internal virtual DelegateValue WithExceededTimeout(TimeSpan timeout, Exception exception)
		=> new(exception, Duration, IsNull)
		{
			ExceededTimeout = timeout,
		};

	/// <inheritdoc />
	public override string ToString()
	{
		if (Exception == null)
		{
			return
				$"delegate returning in {Formatter.Format(Duration)}";
		}

		return
			$"delegate throwing {Formatter.Format(Exception)} after {Formatter.Format(Duration)}";
	}
}
