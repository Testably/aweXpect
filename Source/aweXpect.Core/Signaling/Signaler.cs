using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
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
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public SignalerResult Wait(
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitFor(1, timeout, cancellationToken);
	}

	/// <summary>
	///     Blocks the current thread until the callback was executed at least the required <paramref name="amount" /> of times
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public SignalerResult Wait(Times amount, TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitFor(amount.Value, timeout, cancellationToken);
	}

	/// <summary>
	///     Waits without blocking a thread until the callback was executed at least once
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public Task<SignalerResult> WaitAsync(
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitForAsync(1, timeout, cancellationToken);
	}

	/// <summary>
	///     Waits without blocking a thread until the callback was executed at least the required
	///     <paramref name="amount" /> of times
	///     or the <paramref name="timeout" /> expired
	///     or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public Task<SignalerResult> WaitAsync(Times amount, TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (amount.Value <= 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero."));
		}

		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitForAsync(amount.Value, timeout, cancellationToken);
	}

	private SignalerResult WaitFor(int amount, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		Waiter? waiter = Register(amount, out int counter);
		if (waiter is null)
		{
			return new SignalerResult(true, counter);
		}

		try
		{
			TimeSpan waitTimeout = timeout ?? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (waitTimeout != TimeSpan.Zero)
			{
				SignalWait.Block(waiter.Completion.Task, waitTimeout, cancellationToken);
			}
		}
		finally
		{
			Unregister(waiter);
		}

		return new SignalerResult(waiter.Missing == 0, waiter.Count);
	}

	private async Task<SignalerResult> WaitForAsync(int amount, TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		Waiter? waiter = Register(amount, out int counter);
		if (waiter is null)
		{
			return new SignalerResult(true, counter);
		}

		try
		{
			TimeSpan waitTimeout = timeout ?? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (waitTimeout != TimeSpan.Zero)
			{
				await SignalWait.WaitAsync(waiter.Completion.Task, waitTimeout, cancellationToken);
			}
		}
		finally
		{
			Unregister(waiter);
		}

		return new SignalerResult(waiter.Missing == 0, waiter.Count);
	}

	/// <summary>
	///     Registers a waiter for the <paramref name="amount" /> of signals, or returns <see langword="null" />
	///     when they were already received.
	/// </summary>
	private Waiter? Register(int amount, out int counter)
	{
		lock (_lock)
		{
			counter = _counter;
			if (_counter >= amount)
			{
				return null;
			}

			Waiter waiter = new(amount - _counter);
			_waiters.Add(waiter);
			return waiter;
		}
	}

	private void Unregister(Waiter waiter)
	{
		lock (_lock)
		{
			_waiters.Remove(waiter);
			waiter.Count = _counter;
		}
	}

	/// <remarks>
	///     Each wait has its own waiter, so that concurrent waits do not interfere with each other.
	///     Its state must only be changed under the lock of the signaler.
	/// </remarks>
	private sealed class Waiter(int missing)
	{
		public TaskCompletionSource<bool> Completion { get; } = SignalWait.CreateCompletion();

		/// <summary>
		///     The number of signals when the wait ended.
		/// </summary>
		public int Count { get; set; }

		public int Missing { get; private set; } = missing;

		public void Receive()
		{
			if (Missing > 0 && --Missing == 0)
			{
				Completion.TrySetResult(true);
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
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public SignalerResult<TParameter> Wait(
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitFor(1, predicate, timeout, cancellationToken);
	}

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
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
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

		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitFor(amount.Value, predicate, timeout, cancellationToken);
	}

	/// <summary>
	///     Waits without blocking a thread until<br />
	///     - the callback was executed at least once matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public Task<SignalerResult<TParameter>> WaitAsync(
		Func<TParameter, bool>? predicate = null,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitForAsync(1, predicate, timeout, cancellationToken);
	}

	/// <summary>
	///     Waits without blocking a thread until<br />
	///     - the callback was executed at least the required <paramref name="amount" /> of times
	///     matching the <paramref name="predicate" /><br />
	///     - or the <paramref name="timeout" /> expired<br />
	///     - or the <paramref name="cancellationToken" /> was canceled.
	/// </summary>
	/// <remarks>
	///     If no <paramref name="predicate" /> is provided, all signals are counted.
	///     If no <paramref name="timeout" /> is specified (set to <see langword="null" />),
	///     the <see cref="AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" /> is used
	///     (30 seconds unless customized).
	///     An exception of the <paramref name="predicate" /> ends the wait and is thrown, also when it was thrown while
	///     another thread signaled.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public Task<SignalerResult<TParameter>> WaitAsync(
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

		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return WaitForAsync(amount.Value, predicate, timeout, cancellationToken);
	}

	private SignalerResult<TParameter> WaitFor(int amount, Func<TParameter, bool>? predicate, TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		Waiter waiter = Register(amount, predicate);
		try
		{
			TimeSpan waitTimeout = timeout ?? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (waitTimeout != TimeSpan.Zero)
			{
				SignalWait.Block(waiter.Completion.Task, waitTimeout, cancellationToken);
			}
		}
		finally
		{
			Unregister(waiter);
		}

		return GetResult(waiter);
	}

	private async Task<SignalerResult<TParameter>> WaitForAsync(int amount, Func<TParameter, bool>? predicate,
		TimeSpan? timeout, CancellationToken cancellationToken)
	{
		Waiter waiter = Register(amount, predicate);
		try
		{
			TimeSpan waitTimeout = timeout ?? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();
			if (waitTimeout != TimeSpan.Zero)
			{
				await SignalWait.WaitAsync(waiter.Completion.Task, waitTimeout, cancellationToken);
			}
		}
		finally
		{
			Unregister(waiter);
		}

		return GetResult(waiter);
	}

	/// <remarks>
	///     Registering the waiter together with the snapshot lets every signal count exactly once: either from the
	///     snapshot or from the signal itself.
	/// </remarks>
	private Waiter Register(int amount, Func<TParameter, bool>? predicate)
	{
		Waiter waiter = new(amount, predicate);
		TParameter[] parameters;
		lock (_lock)
		{
			parameters = _parameters.ToArray();
			_waiters.Add(waiter);
		}

		ReceiveOutsideTheLock(waiter, parameters);
		return waiter;
	}

	private void Unregister(Waiter waiter)
	{
		lock (_lock)
		{
			_waiters.Remove(waiter);
			waiter.HasEnded = true;
			waiter.Parameters = _parameters.ToArray();
		}
	}

	private static SignalerResult<TParameter> GetResult(Waiter waiter)
	{
		if (waiter.Exception is not null)
		{
			ExceptionDispatchInfo.Capture(waiter.Exception).Throw();
		}

		return new SignalerResult<TParameter>(waiter.Missing == 0, waiter.Parameters);
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
		public TaskCompletionSource<bool> Completion { get; } = SignalWait.CreateCompletion();
		public Exception? Exception { get; private set; }

		/// <summary>
		///     An ended wait has returned its result, so it must no longer receive signals.
		/// </summary>
		public bool HasEnded { get; set; }

		/// <summary>
		///     The parameters of all signals when the wait ended.
		/// </summary>
		public TParameter[] Parameters { get; set; } = [];

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
				Completion.TrySetResult(true);
			}
		}

		public void Fail(Exception exception)
		{
			if (HasEnded)
			{
				return;
			}

			Exception ??= exception;
			Completion.TrySetResult(true);
		}
	}
}
