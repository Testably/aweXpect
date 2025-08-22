using System;
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the generic arguments.
/// </summary>
public class GenericArgumentFilterOptions(Func<Type, bool> predicate, Func<string> description)
{
	private readonly List<Func<string>> _descriptions = [description,];
	private readonly List<Func<Type, bool>> _predicates = [predicate,];

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<Type, bool> predicate, Func<string> description)
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="argument" /> matches all predicates.
	/// </summary>
	public bool Matches(Type argument)
	{
		if (_predicates.Count == 0)
		{
			return true;
		}

		return _predicates.All(predicate => predicate(argument));
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));
}
