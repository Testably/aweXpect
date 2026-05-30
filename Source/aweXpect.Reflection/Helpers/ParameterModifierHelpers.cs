using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Helper to append a parameter modifier predicate to an existing parameter filter/assertion result.
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
		where TResult : IOptionsProvider<ParameterFilterOptions>
	{
		result.Options.AddModifier(predicate, () => description);
		return result;
	}
}
