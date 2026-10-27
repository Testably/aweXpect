using System;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     The predicates that filter the signals of a <see cref="Signaler{TParameter}" />.
/// </summary>
/// <remarks>
///     Immutable: <see cref="AddPredicate(Func{TParameter, bool}, string)" /> returns new options, so that
///     <see cref="Default" /> can be shared.<br />
///     The timeout is in the <see cref="SignalerOptions" />.
/// </remarks>
[ImmutableOptions]
public sealed class SignalerPredicateOptions<TParameter>
{
	private readonly Func<TParameter, bool>[] _predicates;
	private readonly string _text;

	private SignalerPredicateOptions(Func<TParameter, bool>[] predicates, string text)
	{
		_predicates = predicates;
		_text = text;
	}

	/// <summary>
	///     The options without a predicate, which every signal matches.
	/// </summary>
	public static SignalerPredicateOptions<TParameter> Default { get; } = new([], "");

	/// <summary>
	///     Add a predicate to the signaler options.
	/// </summary>
	/// <remarks>
	///     The predicates of several calls are combined, so that a signal has to match all of them.
	/// </remarks>
	public SignalerPredicateOptions<TParameter> AddPredicate(Func<TParameter, bool> predicate,
		string predicateExpression)
	{
		Func<TParameter, bool>[] predicates = new Func<TParameter, bool>[_predicates.Length + 1];
		_predicates.CopyTo(predicates, 0);
		predicates[_predicates.Length] = predicate;
		string separator = _predicates.Length == 0 ? " with " : " and with ";
		return new SignalerPredicateOptions<TParameter>(predicates, _text + separator + predicateExpression);
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
		foreach (Func<TParameter, bool> predicate in _predicates)
		{
			if (!predicate(parameter))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString() => _text;
}
