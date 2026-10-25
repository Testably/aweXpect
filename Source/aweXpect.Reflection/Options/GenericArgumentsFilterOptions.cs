using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional <see cref="GenericArgumentFilterOptions" /> to filter the generic arguments.
/// </summary>
internal class GenericArgumentsFilterOptions
{
	private readonly List<Func<string>> _descriptions = [];
	private readonly Dictionary<GenericArgumentFilterOptions, OptionsSlot<CollectionIndexOptions>> _filters = [];
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
	/// <remarks>
	///     The index <paramref name="options" /> are read when the filter matches, so that an index option set after
	///     adding the filter applies.
	/// </remarks>
	public void AddFilter(GenericArgumentFilterOptions filter, OptionsSlot<CollectionIndexOptions> options)
		=> _filters.Add(filter, options);

	/// <summary>
	///     Verifies that the generic type arguments of the <paramref name="type" /> matches all predicates.
	/// </summary>
	public ValueTask<bool>
		Matches(Type? type)
	{
		if (type?.IsGenericType != true)
		{
			return new ValueTask<bool>(false);
		}

		return MatchesPredicatesAndFilters(type.GetGenericArguments(),
			type.IsGenericTypeDefinition
				? null
				: type.GetGenericTypeDefinition().GetGenericArguments().Select(x => x.Name).ToArray());
	}

	/// <summary>
	///     Verifies that the generic type arguments of the <paramref name="method" /> matches all predicates.
	/// </summary>
	public ValueTask<bool>
		Matches(MethodInfo? method)
	{
		if (method?.IsGenericMethod != true)
		{
			return new ValueTask<bool>(false);
		}

		return MatchesPredicatesAndFilters(method.GetGenericArguments(), null);
	}

	private async ValueTask<bool>
		MatchesPredicatesAndFilters(Type[] arguments, string[]? genericTypeNames)
	{
		if (_predicates.Count == 0 && _filters.Count == 0)
		{
			return true;
		}

		return MatchesPredicates(arguments) && await MatchesFilters(arguments, genericTypeNames);
	}

	private bool MatchesPredicates(Type[] arguments)
		=> _predicates.All(predicate => UserCode.Invoke(predicate, arguments, "the predicate"));

#if NET8_0_OR_GREATER
	private async ValueTask<bool> MatchesFilters(Type[] arguments, string[]? genericTypeNames)
	{
		foreach (var (filter, collectionIndexOptionsSlot) in _filters)
		{
			CollectionIndexOptions collectionIndexOptions = collectionIndexOptionsSlot.Value;
#else
	private async ValueTask<bool> MatchesFilters(Type[] arguments, string[]? genericTypeNames)
	{
		foreach (KeyValuePair<GenericArgumentFilterOptions, OptionsSlot<CollectionIndexOptions>> keyItem in _filters)
		{
			GenericArgumentFilterOptions filter = keyItem.Key;
			CollectionIndexOptions collectionIndexOptions = keyItem.Value.Value;
#endif
			if (!await arguments.AnyAsync(async (p, i) =>
			    {
				    return collectionIndexOptions.MatchesIndex(i, arguments.Length) == true &&
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
			=> $"with argument {x.Key.GetDescription()}{x.Value.Value.Match.GetDescription()}"));
		if (descriptions.Count == 0)
		{
			return string.Empty;
		}

		return $" {string.Join(" and ", descriptions)}";
	}
}
