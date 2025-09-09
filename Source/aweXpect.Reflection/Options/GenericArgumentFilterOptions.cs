using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the generic arguments.
/// </summary>
public class GenericArgumentFilterOptions
{
	private readonly List<Func<string>> _descriptions;
#if NET8_0_OR_GREATER
	private readonly List<Func<Type, string?, ValueTask<bool>>> _predicates;
#else
	private readonly List<Func<Type, string?, Task<bool>>> _predicates;
#endif

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, ValueTask<bool>> predicate, Func<string> description)
	{
		_descriptions = [description,];
		_predicates = [predicate,];
	}
#else
	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, Task<bool>> predicate, Func<string> description)
	{
		_descriptions = [description,];
		_predicates = [predicate,];
	}
#endif

	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, bool> predicate, Func<string> description)
	{
		_descriptions = [description,];
		_predicates = [ToAsyncPredicate(predicate),];
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<Type, string?, bool> predicate, Func<string> description)
	{
		_predicates.Add(ToAsyncPredicate(predicate));
		_descriptions.Add(description);
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
#if NET8_0_OR_GREATER
	public void AddPredicate(Func<Type, string?, ValueTask<bool>> predicate, Func<string> description)
#else
	public void AddPredicate(Func<Type, string?, Task<bool>> predicate, Func<string> description)
#endif
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="argument" /> matches all predicates.
	/// </summary>
#if NET8_0_OR_GREATER
	public ValueTask<bool> Matches(Type argument, string? genericArgumentName = null)
#else
	public Task<bool> Matches(Type argument, string? genericArgumentName = null)
#endif
	{
		if (_predicates.Count == 0)
		{
#if NET8_0_OR_GREATER
			return ValueTask.FromResult(true);
#else
			return Task.FromResult(true);
#endif
		}

		return _predicates.AllAsync(predicate => predicate(argument, genericArgumentName));
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));

#if NET8_0_OR_GREATER
	private static Func<Type, string?, ValueTask<bool>> ToAsyncPredicate(Func<Type, string?, bool> predicate)
		=> (type, name) => ValueTask.FromResult(predicate(type, name));
#else
	private static Func<Type, string?, Task<bool>> ToAsyncPredicate(Func<Type, string?, bool> predicate)
		=> (type, name) => Task.FromResult(predicate(type, name));
#endif
}
