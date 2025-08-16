using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
public class ParameterFilterOptions(Func<ParameterInfo, bool> predicate, Func<string> description)
{
	private readonly List<Func<string>> _descriptions = [description,];
	private readonly List<Func<ParameterInfo, bool>> _predicates = [predicate,];

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<ParameterInfo, bool> predicate, Func<string> description)
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="parameter" /> matches all predicates.
	/// </summary>
	public bool Matches(ParameterInfo parameter)
	{
		if (_predicates.Count == 0)
		{
			return true;
		}

		return _predicates.All(predicate => predicate(parameter));
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));
}
