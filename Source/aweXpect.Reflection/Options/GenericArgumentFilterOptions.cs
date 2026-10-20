using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the generic arguments.
/// </summary>
public class GenericArgumentFilterOptions
{
	private readonly List<Func<string>> _descriptions;
	private readonly List<Func<Type, string?, ValueTask<bool>>> _predicates;

	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, ValueTask<bool>> predicate, Func<string> description)
	{
		_descriptions = [description,];
		_predicates = [predicate,];
	}

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
	public void AddPredicate(Func<Type, string?, ValueTask<bool>> predicate, Func<string> description)
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="argument" /> matches all predicates.
	/// </summary>
	public ValueTask<bool> Matches(Type argument, string? genericArgumentName = null)
	{
		if (_predicates.Count == 0)
		{
			return new ValueTask<bool>(true);
		}

		return _predicates.AllAsync(predicate
			=> UserCode.InvokeAsync(() => predicate(argument, genericArgumentName), "the predicate"));
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));

	private static Func<Type, string?, ValueTask<bool>> ToAsyncPredicate(Func<Type, string?, bool> predicate)
		=> (type, name) => new ValueTask<bool>(predicate(type, name));
}
