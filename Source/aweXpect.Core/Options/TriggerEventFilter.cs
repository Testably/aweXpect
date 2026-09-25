using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     Filter for event parameters.
/// </summary>
public class TriggerEventFilter
{
	private readonly List<Func<object?[], bool>> _predicates = new();
	private readonly StringBuilder _toString = new();

	/// <summary>
	///     Filters the event parameters using the <paramref name="predicate" />.
	///     The <paramref name="predicateExpression" /> is used in the expectation string.
	/// </summary>
	public void AddPredicate(Func<object?[], bool> predicate, string predicateExpression)
	{
		// ReSharper disable once LocalizableElement
		_ = predicate ?? throw Tracing.WriteException(
			new ArgumentNullException(nameof(predicate), "The 'predicate' cannot be null."));
		if (_predicates.Count != 0)
		{
			_toString.Append(" and");
		}

		_toString.Append(predicateExpression);
		_predicates.Add(predicate);
	}

	/// <summary>
	///     Checks if the provided <paramref name="parameters" /> match all registered predicates.
	/// </summary>
	public bool IsMatch(object?[] parameters)
		=> _predicates.All(predicate => predicate(parameters));

	/// <summary>
	///     A string representation including all predicate expressions.
	/// </summary>
	public override string ToString() => _toString.ToString();
}
