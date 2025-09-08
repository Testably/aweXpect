using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Options;

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

/// <summary>
///     Options for adding additional <see cref="GenericArgumentFilterOptions" /> to filter the generic arguments.
/// </summary>
public class GenericArgumentsFilterOptions
{
	private readonly List<Func<string>> _descriptions = [];
	private readonly Dictionary<GenericArgumentFilterOptions, CollectionIndexOptions> _filters = [];
	private readonly List<Func<Type[], bool>> _predicates = [];

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<Type[], bool> predicate, Func<string> description)
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Adds an additional <see cref="GenericArgumentFilterOptions" />.
	/// </summary>
	public void AddFilter(GenericArgumentFilterOptions filter, CollectionIndexOptions options)
		=> _filters.Add(filter, options);

	/// <summary>
	///     Verifies that the <paramref name="arguments" /> matches all predicates.
	/// </summary>
	public bool Matches(Type[] arguments)
	{
		if (_predicates.Count == 0 && _filters.Count == 0)
		{
			return true;
		}

		if (_predicates.All(predicate => predicate(arguments)))
		{
#if NET8_0_OR_GREATER
			foreach (var (filter, collectionIndexOptions) in _filters)
			{
#else
			foreach (KeyValuePair<GenericArgumentFilterOptions, CollectionIndexOptions> keyItem in _filters)
			{
				GenericArgumentFilterOptions filter = keyItem.Key;
				CollectionIndexOptions collectionIndexOptions = keyItem.Value;
#endif
				if (!arguments.Where((p, i) =>
				    {
					    bool? isIndexInRange = collectionIndexOptions.Match switch
					    {
						    CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						    CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, arguments.Length),
						    _ => false,
					    };
					    return isIndexInRange == true &&
					           filter.Matches(p);
				    }).Any())
				{
					return false;
				}
			}

			return true;
		}

		return false;
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
	{
		List<string> descriptions = _descriptions.Select(@delegate => @delegate()).ToList();
		descriptions.AddRange(_filters.Select(x => $"with argument {x.Key.GetDescription()}{x.Value.Match.GetDescription()}"));
		if (descriptions.Count == 0)
		{
			return string.Empty;
		}

		return $" {string.Join(" and ", descriptions)}";
	}
}
