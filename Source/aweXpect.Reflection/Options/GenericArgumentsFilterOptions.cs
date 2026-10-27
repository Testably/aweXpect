using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional <see cref="GenericArgumentFilterOptions" /> to filter the generic arguments.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="WithPredicate" /> and <see cref="WithFilter" /> return new options. Start from
///     <see cref="Default" />.
/// </remarks>
internal sealed class GenericArgumentsFilterOptions
{
	private readonly Func<string>[] _descriptions;
	private readonly ArgumentFilter[] _filters;
	private readonly Func<Type[], bool>[] _predicates;

	private GenericArgumentsFilterOptions(Func<Type[], bool>[] predicates, Func<string>[] descriptions,
		ArgumentFilter[] filters)
	{
		_predicates = predicates;
		_descriptions = descriptions;
		_filters = filters;
	}

	/// <summary>
	///     The options without any predicate or filter.
	/// </summary>
	public static GenericArgumentsFilterOptions Default { get; } = new([], [], []);

	/// <summary>
	///     Returns options with the additional <paramref name="predicate" /> and <paramref name="description" />.
	/// </summary>
	public GenericArgumentsFilterOptions WithPredicate(Func<Type[], bool> predicate, Func<string> description)
		=> new([.. _predicates, predicate,], [.. _descriptions, description,], _filters);

	/// <summary>
	///     Returns options with the additional <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     The index <paramref name="options" /> are read when the filter matches, so that an index option set after
	///     adding the filter applies.
	/// </remarks>
	public GenericArgumentsFilterOptions WithFilter(GenericArgumentFilterOptions filter,
		IReadOnlyOptionsHolder<CollectionIndexOptions> options)
		=> new(_predicates, _descriptions, [.. _filters, new ArgumentFilter(filter, options),]);

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
		if (_predicates.Length == 0 && _filters.Length == 0)
		{
			return true;
		}

		return MatchesPredicates(arguments) && await MatchesFilters(arguments, genericTypeNames);
	}

	private bool MatchesPredicates(Type[] arguments)
		=> _predicates.All(predicate => UserCode.Invoke(predicate, arguments, "the predicate"));

	private async ValueTask<bool> MatchesFilters(Type[] arguments, string[]? genericTypeNames)
	{
		foreach (ArgumentFilter argumentFilter in _filters)
		{
			GenericArgumentFilterOptions filter = argumentFilter.Filter;
			CollectionIndexOptions collectionIndexOptions = argumentFilter.IndexOptions.Value;
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
			=> $"with argument {x.Filter.GetDescription()}{x.IndexOptions.Value.Match.GetDescription()}"));
		if (descriptions.Count == 0)
		{
			return string.Empty;
		}

		return $" {string.Join(" and ", descriptions)}";
	}

	private sealed class ArgumentFilter(
		GenericArgumentFilterOptions filter,
		IReadOnlyOptionsHolder<CollectionIndexOptions> indexOptions)
	{
		public GenericArgumentFilterOptions Filter { get; } = filter;
		public IReadOnlyOptionsHolder<CollectionIndexOptions> IndexOptions { get; } = indexOptions;
	}
}
