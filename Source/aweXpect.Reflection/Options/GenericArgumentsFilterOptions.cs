using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

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
#if NET8_0_OR_GREATER
	public ValueTask<bool>
#else
	public Task<bool>
#endif
		Matches(Type? type)
	{
		if (type?.IsGenericType != true)
		{
#if NET8_0_OR_GREATER
			return ValueTask.FromResult(false);
#else
			return Task.FromResult(false);
#endif
		}

		return MatchesPredicatesAndFilters(type.GetGenericArguments(),
			type.IsGenericTypeDefinition
				? null
				: type.GetGenericTypeDefinition().GetGenericArguments().Select(x => x.Name).ToArray());
	}

	/// <summary>
	///     Verifies that the generic type arguments of the <paramref name="method" /> matches all predicates.
	/// </summary>
#if NET8_0_OR_GREATER
	public ValueTask<bool>
#else
	public Task<bool>
#endif
		Matches(MethodInfo? method)
	{
		if (method?.IsGenericMethod != true)
		{
#if NET8_0_OR_GREATER
			return ValueTask.FromResult(false);
#else
			return Task.FromResult(false);
#endif
		}

		return MatchesPredicatesAndFilters(method.GetGenericArguments(), null);
	}

#if NET8_0_OR_GREATER
	private async ValueTask<bool>
#else
	private async Task<bool>
#endif
		MatchesPredicatesAndFilters(Type[] arguments, string[]? genericTypeNames)
	{
		if (_predicates.Count == 0 && _filters.Count == 0)
		{
			return true;
		}

		return MatchesPredicates(arguments) && await MatchesFilters(arguments, genericTypeNames);
	}

	private bool MatchesPredicates(Type[] arguments)
		=> _predicates.All(predicate => predicate(arguments));

#if NET8_0_OR_GREATER
	private async ValueTask<bool> MatchesFilters(Type[] arguments, string[]? genericTypeNames)
	{
		foreach (var (filter, collectionIndexOptions) in _filters)
		{
#else
	private async Task<bool> MatchesFilters(Type[] arguments, string[]? genericTypeNames)
	{
		foreach (KeyValuePair<GenericArgumentFilterOptions, CollectionIndexOptions> keyItem in _filters)
		{
			GenericArgumentFilterOptions filter = keyItem.Key;
			CollectionIndexOptions collectionIndexOptions = keyItem.Value;
#endif
			if (!await arguments.AnyAsync(async (p, i) =>
			    {
				    bool? isIndexInRange = collectionIndexOptions.Match switch
				    {
					    CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
					    CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, arguments.Length),
					    _ => false,
				    };
				    return isIndexInRange == true &&
				           await filter.Matches(p, genericTypeNames?.Length > i ? genericTypeNames[i] : null);
			    }))
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
