using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
/// <remarks>
///     An immutable value: the <c>With…</c> methods return new options and leave these unchanged.
/// </remarks>
internal sealed class ParameterFilterOptions
{
	private readonly Func<string>[] _descriptions;
	private readonly Func<string>[] _modifierDescriptions;
	private readonly Func<ParameterInfo, ValueTask<bool>>[] _predicates;

	/// <inheritdoc cref="ParameterFilterOptions" />
	public ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>> predicate, Func<string> description)
		: this([predicate,], [description,], [])
	{
	}

	/// <inheritdoc cref="ParameterFilterOptions" />
	public ParameterFilterOptions(Func<ParameterInfo, bool> predicate, Func<string> description)
		: this([ToAsyncPredicate(predicate),], [description,], [])
	{
	}

	/// <inheritdoc cref="ParameterFilterOptions" />
	internal ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>> predicate)
		: this([predicate,], [], [])
	{
	}

	/// <inheritdoc cref="ParameterFilterOptions" />
	internal ParameterFilterOptions(Func<ParameterInfo, bool> predicate)
		: this([ToAsyncPredicate(predicate),], [], [])
	{
	}

	private ParameterFilterOptions(Func<ParameterInfo, ValueTask<bool>>[] predicates, Func<string>[] descriptions,
		Func<string>[] modifierDescriptions)
	{
		_predicates = predicates;
		_descriptions = descriptions;
		_modifierDescriptions = modifierDescriptions;
	}

	/// <summary>
	///     Returns options with an additional <paramref name="predicate" /> and <paramref name="description" />.
	/// </summary>
	internal ParameterFilterOptions WithPredicate(Func<ParameterInfo, bool> predicate, Func<string> description)
		=> WithPredicate(ToAsyncPredicate(predicate), description);

	/// <summary>
	///     Returns options with an additional <paramref name="predicate" /> and <paramref name="description" />.
	/// </summary>
	internal ParameterFilterOptions WithPredicate(Func<ParameterInfo, ValueTask<bool>> predicate,
		Func<string> description)
		=> new([.. _predicates, predicate,], [.. _descriptions, description,], _modifierDescriptions);

	/// <summary>
	///     Returns options with an additional <paramref name="predicate" /> without a description.
	/// </summary>
	internal ParameterFilterOptions WithPredicate(Func<ParameterInfo, bool> predicate)
		=> WithPredicate(ToAsyncPredicate(predicate));

	/// <summary>
	///     Returns options with an additional <paramref name="predicate" /> without a description.
	/// </summary>
	internal ParameterFilterOptions WithPredicate(Func<ParameterInfo, ValueTask<bool>> predicate)
		=> new([.. _predicates, predicate,], _descriptions, _modifierDescriptions);

	/// <summary>
	///     Returns options with an additional modifier <paramref name="predicate" /> and <paramref name="description" />.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="WithPredicate(Func{ParameterInfo, bool}, Func{string})" />, the
	///     <paramref name="description" /> is additionally tracked separately so that it can be appended to assertion
	///     expectations via <see cref="GetModifierDescription" />.
	/// </remarks>
	internal ParameterFilterOptions WithModifier(Func<ParameterInfo, bool> predicate, Func<string> description)
		=> new([.. _predicates, ToAsyncPredicate(predicate),], _descriptions, [.. _modifierDescriptions, description,]);

	/// <summary>
	///     Verifies that the <paramref name="parameter" /> matches all predicates.
	/// </summary>
	public ValueTask<bool> Matches(ParameterInfo parameter)
	{
		if (_predicates.Length == 0)
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
		=> _modifierDescriptions.Length == 0
			? string.Empty
			: " " + string.Join(" and ", _modifierDescriptions.Select(@delegate => @delegate()));
}
