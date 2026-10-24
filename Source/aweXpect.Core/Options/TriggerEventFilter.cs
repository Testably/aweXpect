using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     Filter for event parameters.
/// </summary>
public class TriggerEventFilter
{
	private List<Func<object?[], bool>>? _predicates;
	private StringBuilder? _toString;

	/// <summary>
	///     Filters the event parameters using the <paramref name="predicate" />.
	///     The <paramref name="predicateExpression" /> is used in the expectation string.
	/// </summary>
	public TriggerEventFilter AddPredicate(Func<object?[], bool> predicate, string predicateExpression)
	{
		// ReSharper disable once LocalizableElement
		_ = predicate ?? throw Tracing.WriteException(
			new ArgumentNullException(nameof(predicate), "The 'predicate' cannot be null."));
		_toString ??= new StringBuilder();
		if (_predicates is not null)
		{
			_toString.Append(" and");
		}

		_toString.Append(predicateExpression);
		(_predicates ??= []).Add(predicate);
		return this;
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
		if (_predicates is null)
		{
			return true;
		}

		for (int i = 0; i < _predicates.Count; i++)
		{
			if (!_predicates[i](parameters))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     A string representation including all predicate expressions.
	/// </summary>
	public override string ToString() => _toString?.ToString() ?? "";
}
