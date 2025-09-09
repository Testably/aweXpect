using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Options;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the generic arguments.
/// </summary>
public class GenericArgumentFilterOptions(Func<Type, string?, bool> predicate, Func<string> description)
{
	private readonly List<Func<string>> _descriptions = [description,];
	private readonly List<Func<Type, string?, bool>> _predicates = [predicate,];

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<Type, string?, bool> predicate, Func<string> description)
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="argument" /> matches all predicates.
	/// </summary>
	public bool Matches(Type argument, string? genericArgumentName = null)
	{
		if (_predicates.Count == 0)
		{
			return true;
		}

		return _predicates.All(predicate => predicate(argument, genericArgumentName));
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
	///     Verifies that the generic type arguments of the <paramref name="type" /> matches all predicates.
	/// </summary>
	public bool Matches(Type type) => MatchesPredicatesAndFilters(type.GetGenericArguments(),
		type.IsGenericTypeDefinition
			? null
			: type.GetGenericTypeDefinition().GetGenericArguments().Select(x => x.Name).ToArray());

	/// <summary>
	///     Verifies that the generic type arguments of the <paramref name="method" /> matches all predicates.
	/// </summary>
	public bool Matches(MethodInfo method) => MatchesPredicatesAndFilters(method.GetGenericArguments(), null);

	private bool MatchesPredicatesAndFilters(Type[] arguments, string[]? genericTypeNames)
	{
		if (_predicates.Count == 0 && _filters.Count == 0)
		{
			return true;
		}

		return MatchesPredicates(arguments) && MatchesFilters(arguments, genericTypeNames);
	}

	private bool MatchesPredicates(Type[] arguments)
		=> _predicates.All(predicate => predicate(arguments));

	private bool MatchesFilters(Type[] arguments, string[]? genericTypeNames)
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
				           filter.Matches(p, genericTypeNames?.Length > i ? genericTypeNames[i] : null);
			    }).Any())
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
	{
		List<string> descriptions = _descriptions.Select(@delegate => @delegate()).ToList();
		descriptions.AddRange(_filters.Select(x
			=> $"with argument {x.Key.GetDescription()}{x.Value.Match.GetDescription()}"));
		if (descriptions.Count == 0)
		{
			return string.Empty;
		}

		return $" {string.Join(" and ", descriptions)}";
	}
}
