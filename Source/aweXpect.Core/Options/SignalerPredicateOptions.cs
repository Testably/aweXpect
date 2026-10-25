using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     The predicates that filter the signals of a <see cref="Signaler{TParameter}" />.
/// </summary>
/// <remarks>
///     The timeout is in the <see cref="SignalerOptions" />.
/// </remarks>
public class SignalerPredicateOptions<TParameter>
{
	private StringBuilder? _builder;
	private List<Func<TParameter, bool>>? _predicates;

	/// <summary>
	///     Add a predicate to the signaler options.
	/// </summary>
	/// <remarks>
	///     The predicates of several calls are combined, so that a signal has to match all of them.
	/// </remarks>
	public SignalerPredicateOptions<TParameter> WithPredicate(Func<TParameter, bool> predicate,
		string predicateExpression)
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
		=> _builder?.ToString() ?? "";
}
