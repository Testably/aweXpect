using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
public class ParameterFilterOptions
{
	private readonly List<Func<string>> _descriptions;
#if NET8_0_OR_GREATER
	private readonly List<Func<ParameterInfo, ValueTask<bool>>> _predicates;
#else
	private readonly List<Func<ParameterInfo, Task<bool>>> _predicates;
#endif

	/// <inheritdoc cref="ParameterFilterOptions" />
#if NET8_0_OR_GREATER
	public ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>> predicate, Func<string> description)
#else
	public ParameterFilterOptions(Func<ParameterInfo, Task<bool>> predicate, Func<string> description)
#endif
	{
		_descriptions = [description,];
		_predicates = [predicate,];
	}

	/// <inheritdoc cref="ParameterFilterOptions" />
	public ParameterFilterOptions(Func<ParameterInfo, bool> predicate, Func<string> description)
	{
		_descriptions = [description,];
		_predicates = [ToAsyncPredicate(predicate),];
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
#if NET8_0_OR_GREATER
	public void AddPredicate(Func<ParameterInfo, ValueTask<bool>> predicate, Func<string> description)
#else
	public void AddPredicate(Func<ParameterInfo, Task<bool>> predicate, Func<string> description)
#endif
	{
		_predicates.Add(predicate);
		_descriptions.Add(description);
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<ParameterInfo, bool> predicate, Func<string> description)
	{
		_predicates.Add(ToAsyncPredicate(predicate));
		_descriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="parameter" /> matches all predicates.
	/// </summary>
#if NET8_0_OR_GREATER
	public ValueTask<bool> Matches(ParameterInfo parameter)
#else
	public Task<bool> Matches(ParameterInfo parameter)
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

		return _predicates.AllAsync(predicate => predicate(parameter));
	}

#if NET8_0_OR_GREATER
	private static Func<ParameterInfo, ValueTask<bool>> ToAsyncPredicate(Func<ParameterInfo, bool> predicate)
		=> p => ValueTask.FromResult(predicate(p));
#else
	private static Func<ParameterInfo, Task<bool>> ToAsyncPredicate(Func<ParameterInfo, bool> predicate)
		=> p => Task.FromResult(predicate(p));
#endif

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));
}
