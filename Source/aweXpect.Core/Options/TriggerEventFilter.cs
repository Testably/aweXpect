using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     Filter for event parameters.
/// </summary>
/// <remarks>
///     Immutable: <see cref="AddPredicate(Func{object?[], bool}, string)" /> returns a new filter, so that
///     <see cref="Default" /> can be shared.
/// </remarks>
[ImmutableOptions]
public sealed class TriggerEventFilter
{
	private readonly Func<object?[], bool>[] _predicates;
	private readonly string _text;

	private TriggerEventFilter(Func<object?[], bool>[] predicates, string text)
	{
		_predicates = predicates;
		_text = text;
	}

	/// <summary>
	///     The filter without a predicate, which every event matches.
	/// </summary>
	public static TriggerEventFilter Default { get; } = new([], "");

	/// <summary>
	///     Returns a filter that also filters the event parameters using the <paramref name="predicate" />.
	///     The <paramref name="predicateExpression" /> is used in the expectation string.
	/// </summary>
	public TriggerEventFilter AddPredicate(Func<object?[], bool> predicate, string predicateExpression)
	{
		// ReSharper disable once LocalizableElement
		_ = predicate ?? throw Tracing.WriteException(
			new ArgumentNullException(nameof(predicate), "The 'predicate' cannot be null."));
		Func<object?[], bool>[] predicates = new Func<object?[], bool>[_predicates.Length + 1];
		_predicates.CopyTo(predicates, 0);
		predicates[_predicates.Length] = predicate;
		string separator = _predicates.Length == 0 ? "" : " and";
		return new TriggerEventFilter(predicates, _text + separator + predicateExpression);
	}

	/// <summary>
	///     Checks if the provided <paramref name="parameters" /> match all registered predicates.
	/// </summary>
	/// <remarks>
	///     A loop instead of <c>All</c>, as a lambda over the <paramref name="parameters" /> would allocate a closure for
	///     every recorded event.
	/// </remarks>
	public bool IsMatch(object?[] parameters)
	{
		foreach (Func<object?[], bool> predicate in _predicates)
		{
			if (!predicate(parameters))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     A string representation including all predicate expressions.
	/// </summary>
	public override string ToString() => _text;
}
