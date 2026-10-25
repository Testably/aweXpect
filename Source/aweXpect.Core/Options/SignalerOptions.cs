using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     Options for <see cref="Signaler" />.
/// </summary>
public class SignalerOptions
{
	/// <summary>
	///     The timeout to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	public TimeSpan? Timeout { get; private set; }

	/// <summary>
	///     Specifies the <paramref name="timeout" /> to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.<br />
	///     A later timeout replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public SignalerOptions Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		Timeout = timeout;
		return this;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
		=> WithinText();

	/// <remarks>
	///     An infinite timeout is omitted, because it does not add any information to the expectation.
	/// </remarks>
	private protected string WithinText()
		=> Timeout is null || Timeout == System.Threading.Timeout.InfiniteTimeSpan
			? ""
			: $" within {Formatter.Format(Timeout.Value)}";
}

/// <summary>
///     Options for <see cref="Signaler{TParameter}" />.
/// </summary>
public class SignalerOptions<TParameter> : SignalerOptions
{
	private StringBuilder? _builder;
	private List<Func<TParameter, bool>>? _predicates;

	/// <summary>
	///     Add a predicate to the signaler options.
	/// </summary>
	/// <remarks>
	///     The predicates of several calls are combined, so that a signal has to match all of them.
	/// </remarks>
	public SignalerOptions<TParameter> WithPredicate(Func<TParameter, bool> predicate, string predicateExpression)
	{
		_predicates ??= new List<Func<TParameter, bool>>();
		_predicates.Add(predicate);
		if (_builder is null)
		{
			_builder = new StringBuilder();
			_builder.Append(" with ");
			_builder.Append(predicateExpression);
		}
		else
		{
			_builder.Append(" and with ");
			_builder.Append(predicateExpression);
		}

		return this;
	}

	/// <summary>
	///     Checks if the <paramref name="parameter" /> matches all registered predicates.
	/// </summary>
	/// <remarks>
	///     A loop instead of <c>All</c>, as a lambda over the <paramref name="parameter" /> would allocate a closure for
	///     every signal.
	/// </remarks>
	public bool Matches(TParameter parameter)
	{
		if (_predicates is null)
		{
			return true;
		}

		for (int i = 0; i < _predicates.Count; i++)
		{
			if (!_predicates[i](parameter))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
		=> $"{_builder}{WithinText()}";
}
