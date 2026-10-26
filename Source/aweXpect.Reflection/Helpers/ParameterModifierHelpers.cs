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
	public static TSelf WithModifier<TSelf>(
		this TSelf result,
		Func<ParameterInfo, bool> predicate,
		string description)
		where TSelf : IParameterFilterOptionsProvider
	{
		result.ParameterFilterOptions.Value =
			result.ParameterFilterOptions.Value.WithModifier(predicate, () => description);
		return result;
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
