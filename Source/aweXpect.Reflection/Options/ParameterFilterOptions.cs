using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
public class ParameterFilterOptions
{
	private readonly List<Func<string>> _descriptions;
	private readonly List<Func<string>> _modifierDescriptions = [];
	private readonly List<Func<ParameterInfo, ValueTask<bool>>> _predicates;

	/// <inheritdoc cref="ParameterFilterOptions" />
	public ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>> predicate, Func<string> description)
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

	/// <inheritdoc cref="ParameterFilterOptions" />
	internal ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>> predicate)
	{
		_descriptions = [];
		_predicates = [predicate,];
	}

	/// <inheritdoc cref="ParameterFilterOptions" />
	internal ParameterFilterOptions(Func<ParameterInfo, bool> predicate)
	{
		_descriptions = [];
		_predicates = [ToAsyncPredicate(predicate),];
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	public void AddPredicate(Func<ParameterInfo, ValueTask<bool>> predicate, Func<string> description)
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
	///     Adds an additional <paramref name="predicate" />.
	/// </summary>
	internal void AddPredicate(Func<ParameterInfo, ValueTask<bool>> predicate)
	{
		_predicates.Add(predicate);
	}

	/// <summary>
	///     Adds an additional <paramref name="predicate" />.
	/// </summary>
	internal void AddPredicate(Func<ParameterInfo, bool> predicate) => _predicates.Add(ToAsyncPredicate(predicate));

	/// <summary>
	///     Adds an additional modifier <paramref name="predicate" /> with the <paramref name="description" />.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="AddPredicate(Func{ParameterInfo, bool}, Func{string})" />, the <paramref name="description" />
	///     is additionally tracked separately so that it can be appended to assertion expectations via
	///     <see cref="GetModifierDescription" />.
	/// </remarks>
	public void AddModifier(Func<ParameterInfo, bool> predicate, Func<string> description)
	{
		_predicates.Add(ToAsyncPredicate(predicate));
		_modifierDescriptions.Add(description);
	}

	/// <summary>
	///     Verifies that the <paramref name="parameter" /> matches all predicates.
	/// </summary>
	public ValueTask<bool> Matches(ParameterInfo parameter)
	{
		if (_predicates.Count == 0)
		{
			return new ValueTask<bool>(true);
		}

		return _predicates.AllAsync(predicate => UserCode.InvokeAsync(() => predicate(parameter), "the predicate"));
	}

	private static Func<ParameterInfo, ValueTask<bool>> ToAsyncPredicate(Func<ParameterInfo, bool> predicate)
		=> p => new ValueTask<bool>(predicate(p));

	/// <summary>
	///     Returns the combination of all descriptions (including modifier descriptions) joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Concat(_modifierDescriptions).Select(@delegate => @delegate()));

	/// <summary>
	///     Returns the combination of all modifier descriptions joined by <c>" and "</c> and prefixed with a space,
	///     or an empty string if no modifier descriptions exist.
	/// </summary>
	public string GetModifierDescription()
		=> _modifierDescriptions.Count == 0
			? string.Empty
			: " " + string.Join(" and ", _modifierDescriptions.Select(@delegate => @delegate()));
}
