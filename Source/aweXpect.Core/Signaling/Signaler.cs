using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Signaling;

/// <summary>
///     Creates a new signaler which receives signals without parameters.
/// </summary>
public class Signaler
{
	private readonly object _lock = new();
	private readonly List<Waiter> _waiters = new();
	private int _counter;

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
			foreach (Waiter waiter in _waiters)
			{
				waiter.Receive();
			}
		}
	}

	/// <summary>
	///     Blocks the current thread until the callback was executed at least once
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	public SignalerResult Wait(
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
		=> WaitFor(1, timeout, cancellationToken);

	/// <summary>
	///     Blocks the current thread until the callback was executed at least the required <paramref name="amount" /> of times
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	public SignalerResult Wait(Times amount, TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		return WaitFor(amount.Value, timeout, cancellationToken);
	}

	private SignalerResult WaitFor(int amount, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		Waiter waiter;
		lock (_lock)
		{
			if (_counter >= amount)
			{
				return new SignalerResult(true, _counter);
			}

			waiter = new Waiter(amount - _counter);
			_waiters.Add(waiter);
		}

		int count;
		try
		{
			timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (timeout != TimeSpan.Zero)
			{
				waiter.Event.Wait(timeout.Value.ToTimerTimeout(), cancellationToken);
			}
		}
		catch (OperationCanceledException)
		{
			// Ignore a cancelled operation
		}
		finally
		{
			lock (_lock)
			{
				_waiters.Remove(waiter);
				count = _counter;
			}

			waiter.Event.Dispose();
		}

		return new SignalerResult(waiter.Missing == 0, count);
	}

	/// <remarks>
	///     Each wait has its own waiter, so that concurrent waits do not interfere with each other.
	///     Its state must only be changed under the lock of the signaler.
	/// </remarks>
	private sealed class Waiter(int missing)
	{
		public ManualResetEventSlim Event { get; } = new();
		public int Missing { get; private set; } = missing;

		public void Receive()
		{
			if (Missing > 0 && --Missing == 0)
			{
				Event.Set();
			}
		}
	}
}

/// <summary>
///     Creates a new signaler which receives signals with parameters of type <typeparamref name="TParameter" />.
/// </summary>
public class Signaler<TParameter>
{
	private readonly object _lock = new();
	private readonly List<TParameter> _parameters = new();
	private readonly List<Waiter> _waiters = new();
	private int _counter;

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
	/// <remarks>
	///     It runs on the thread of the code that signals, so an exception of the predicate of a pending <c>Wait</c>
	///     is not thrown here, but ends the wait and is thrown by the <c>Wait</c> instead.
	/// </remarks>
	public void Signal(TParameter parameter)
	{
		Waiter[] waiters;
		lock (_lock)
		{
			Interlocked.Increment(ref _counter);
			_parameters.Add(parameter);
			waiters = _waiters.ToArray();
		}

		TParameter[] parameters = [parameter,];
		foreach (Waiter waiter in waiters)
		{
			ReceiveOutsideTheLock(waiter, parameters);
		}
	}

	/// <summary>
	///     Blocks the current thread until<br />
	///     - the callback was executed at least once matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	public SignalerResult<TParameter> Wait(
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
		=> WaitFor(1, predicate, timeout, cancellationToken);

	/// <summary>
	///     Blocks the current thread until<br />
	///     - the callback was executed at least the required <paramref name="amount" /> of times
	///     matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	public SignalerResult<TParameter> Wait(
		Times amount,
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		return WaitFor(amount.Value, predicate, timeout, cancellationToken);
	}

	private SignalerResult<TParameter> WaitFor(int amount, Func<TParameter, bool>? predicate, TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		Waiter waiter = new(amount, predicate);
		TParameter[] parameters;
		// Registering the waiter together with the snapshot lets every signal count exactly once: either from the
		// snapshot or from the signal itself.
		lock (_lock)
		{
			parameters = _parameters.ToArray();
			_waiters.Add(waiter);
		}

		try
		{
			ReceiveOutsideTheLock(waiter, parameters);
			timeout ??= Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (timeout != TimeSpan.Zero)
			{
				try
				{
					waiter.Event.Wait(timeout.Value.ToTimerTimeout(), cancellationToken);
				}
				catch (OperationCanceledException)
				{
					// Ignore a cancelled operation
				}
			}
		}
		finally
		{
			lock (_lock)
			{
				_waiters.Remove(waiter);
				waiter.HasEnded = true;
				parameters = _parameters.ToArray();
			}

			waiter.Event.Dispose();
		}

		if (waiter.Exception is not null)
		{
			ExceptionDispatchInfo.Capture(waiter.Exception).Throw();
		}

		return new SignalerResult<TParameter>(waiter.Missing == 0, parameters);
	}

	/// <remarks>
	///     The predicate is user code, so it runs outside the lock: it may signal again or block on another thread
	///     that signals.
	/// </remarks>
	private void ReceiveOutsideTheLock(Waiter waiter, TParameter[] parameters)
	{
		int matches;
		try
		{
			matches = parameters.Count(waiter.Matches);
		}
		catch (Exception exception)
		{
			lock (_lock)
			{
				waiter.Fail(exception);
			}

			return;
		}

		lock (_lock)
		{
			waiter.Receive(matches);
		}
	}

	/// <remarks>
	///     Each wait has its own waiter, so that concurrent waits do not interfere with each other.
	///     Its state must only be changed under the lock of the signaler.
	/// </remarks>
	private sealed class Waiter(int missing, Func<TParameter, bool>? predicate)
	{
		public ManualResetEventSlim Event { get; } = new();
		public Exception? Exception { get; private set; }

		/// <summary>
		///     An ended wait has disposed its <see cref="Event" />, so it must no longer receive signals.
		/// </summary>
		public bool HasEnded { get; set; }

		public int Missing { get; private set; } = missing;

		public bool Matches(TParameter parameter) => predicate?.Invoke(parameter) != false;

		public void Receive(int matches)
		{
			if (HasEnded || Missing == 0 || matches == 0)
			{
				return;
			}

			Missing = Math.Max(0, Missing - matches);
			if (Missing == 0)
			{
				Event.Set();
			}
		}

		public void Fail(Exception exception)
		{
			if (HasEnded)
			{
				return;
			}

			Exception ??= exception;
			Event.Set();
		}
	}
}
