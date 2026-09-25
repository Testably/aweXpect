using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     Options for <see cref="Signaler" />.
/// </summary>
public record SignalerOptions
{
	/// <summary>
	///     The timeout to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	public TimeSpan? Timeout { get; set; }

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
public record SignalerOptions<TParameter> : SignalerOptions
{
	private StringBuilder? _builder;
	private List<Func<TParameter, bool>>? _predicates;

	/// <summary>
	///     Add a predicate to the signaler options.
	/// </summary>
	public void WithPredicate(Func<TParameter, bool> predicate, string predicateExpression)
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
	}

	/// <summary>
	///     Checks if the <paramref name="parameter" /> matches all registered predicates.
	/// </summary>
	public bool Matches(TParameter parameter)
	{
		if (_predicates is null)
		{
			return true;
		}

		return _predicates.All(predicate => predicate(parameter));
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
		=> $"{_builder}{WithinText()}";
}
