using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Signaling;

/// <summary>
///     Creates a new signaler which receives signals without parameters.
/// </summary>
public class Signaler
{
	private readonly object _lock = new();
	private CountdownEvent? _countdownEvent;
	private int _counter;
	private ManualResetEventSlim? _resetEvent;

	/// <summary>
	///     Checks if the callback was signaled at least <paramref name="amount" /> times.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="amount" /> is specified, checks if it was signaled at least once.
	/// </remarks>
	public bool IsSignaled(Times? amount = null)
	{
		int value = amount?.Value ?? 1;
		return _counter >= value;
	}

	/// <summary>
	///     Signals that the callback was executed.
	/// </summary>
	public void Signal()
	{
		lock (_lock)
		{
			Interlocked.Increment(ref _counter);
			_resetEvent?.Set();
			_countdownEvent?.Signal();
		}
	}

	/// <summary>
	///     Blocks the current thread until the callback was executed at least once
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was cancelled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     a default timeout of 30 seconds is used.
	/// </remarks>
	public SignalerResult Wait(
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		lock (_lock)
		{
			if (_counter > 0)
			{
				return new SignalerResult(true, _counter);
			}

			_resetEvent = new ManualResetEventSlim();
		}

		timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
		try
		{
			if (timeout != TimeSpan.Zero && _resetEvent.Wait(timeout.Value, cancellationToken))
			{
				return new SignalerResult(true, _counter);
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore a cancelled operation
		}
		finally
		{
			_resetEvent.Dispose();
		}

		return new SignalerResult(false, _counter);
	}

	/// <summary>
	///     Blocks the current thread until the callback was executed at least the required <paramref name="amount" /> of times
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was cancelled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     a default timeout of 30 seconds is used.
	/// </remarks>
	public SignalerResult Wait(Times amount, TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		lock (_lock)
		{
			if (_counter >= amount.Value)
			{
				return new SignalerResult(true, _counter);
			}

			_countdownEvent = new CountdownEvent(amount.Value - _counter);
		}

		timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
		try
		{
			if (timeout != TimeSpan.Zero && _countdownEvent.Wait(timeout.Value, cancellationToken))
			{
				return new SignalerResult(true, _counter);
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore a cancelled operation
		}
		finally
		{
			_countdownEvent.Dispose();
		}

		return new SignalerResult(false, _counter);
	}
}

/// <summary>
///     Creates a new signaler which receives signals with parameters of type <typeparamref name="TParameter" />.
/// </summary>
public class Signaler<TParameter>
{
	private readonly object _lock = new();
	private readonly List<TParameter> _parameters = new();
	private CountdownEvent? _countdownEvent;
	private int _counter;
	private Func<TParameter, bool>? _predicate;
	private ManualResetEventSlim? _resetEvent;

	/// <summary>
	///     Checks if the callback was signaled at least <paramref name="amount" /> times.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="amount" /> is specified, checks if it was signaled at least once.
	/// </remarks>
	public bool IsSignaled(Times? amount = null)
	{
		int value = amount?.Value ?? 1;
		return _counter >= value;
	}

	/// <summary>
	///     Signals that the callback was executed with the provided <paramref name="parameter" />.
	/// </summary>
	public void Signal(TParameter parameter)
	{
		lock (_lock)
		{
			Interlocked.Increment(ref _counter);
			_parameters.Add(parameter);
			if (_predicate?.Invoke(parameter) != false)
			{
				_resetEvent?.Set();
				_countdownEvent?.Signal();
			}
		}
	}

	/// <summary>
	///     Blocks the current thread until<br />
	///     - the callback was executed at least once matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was cancelled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     a default timeout of 30 seconds is used.
	/// </remarks>
	public SignalerResult<TParameter> Wait(
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		_predicate = predicate;
		lock (_lock)
		{
			if (GetMatchingCount(predicate) == 0)
			{
				_resetEvent = new ManualResetEventSlim();
			}
		}

		timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
		if (timeout != TimeSpan.Zero && _resetEvent != null)
		{
			try
			{
				if (_resetEvent.Wait(timeout.Value, cancellationToken))
				{
					return new SignalerResult<TParameter>(true, _parameters.ToArray());
				}
			}
			catch (OperationCanceledException)
			{
				// Ignore a cancelled operation
			}
			finally
			{
				_resetEvent.Dispose();
			}

			return new SignalerResult<TParameter>(false, _parameters.ToArray());
		}

		return new SignalerResult<TParameter>(GetMatchingCount(predicate) > 0, _parameters.ToArray());
	}

	/// <summary>
	///     Blocks the current thread until<br />
	///     - the callback was executed at least the required <paramref name="amount" /> of times
	///     matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was cancelled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     a default timeout of 30 seconds is used.
	/// </remarks>
	public SignalerResult<TParameter> Wait(
		Times amount,
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)

	{
		_predicate = predicate;
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		lock (_lock)
		{
			int actualCount = GetMatchingCount(predicate);
			if (actualCount >= amount.Value)
			{
				return new SignalerResult<TParameter>(true, _parameters.ToArray());
			}

			_countdownEvent = new CountdownEvent(amount.Value - actualCount);
		}

		timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
		try
		{
			if (timeout != TimeSpan.Zero && _countdownEvent.Wait(timeout.Value, cancellationToken))
			{
				return new SignalerResult<TParameter>(true, _parameters.ToArray());
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore a cancelled operation
		}
		finally
		{
			_countdownEvent.Dispose();
		}

		return new SignalerResult<TParameter>(false, _parameters.ToArray());
	}

	private int GetMatchingCount(Func<TParameter, bool>? predicate)
	{
		if (predicate is null)
		{
			return _parameters.Count;
		}

		return _parameters.Count(predicate);
	}
}
