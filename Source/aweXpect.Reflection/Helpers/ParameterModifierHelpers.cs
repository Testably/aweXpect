using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Helper to append a parameter modifier predicate to an existing parameter assertion result.
/// </summary>
internal static class ParameterModifierHelpers
{
	/// <summary>
	///     Adds the modifier <paramref name="predicate" /> with the <paramref name="description" /> to the
	///     <see cref="ParameterFilterOptions" /> of the <paramref name="result" /> and returns it for further chaining.
	/// </summary>
	public static TResult WithModifier<TResult>(
		this TResult result,
		Func<ParameterInfo, bool> predicate,
		string description)
		where TResult : IParameterFilterOptionsProvider
	{
		result.ParameterFilterOptions.AddModifier(predicate, () => description);
		return result;
	}

	/// <summary>
	///     Adds the custom <paramref name="predicate" /> to the <paramref name="options" />, described by its caller
	///     argument <paramref name="expression" />.
	/// </summary>
	public static void AddMatching(
		this ParameterFilterOptions options,
		Func<ParameterInfo, bool> predicate,
		string expression)
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		options.AddModifier(predicate, () => $"matching {expression.TrimExpression()}");
	}

	/// <summary>
	///     Returns a copy of the <paramref name="options" /> with the custom <paramref name="predicate" />, described by
	///     its caller argument <paramref name="expression" />, while the <paramref name="options" /> stay unchanged.
	/// </summary>
	public static ParameterFilterOptions WithMatching(
		this ParameterFilterOptions options,
		Func<ParameterInfo, bool> predicate,
		string expression)
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return options.WithModifier(predicate, () => $"matching {expression.TrimExpression()}");
	}
}
