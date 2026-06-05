using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Excludes assemblies that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all assemblies matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Assemblies Except(this Filtered.Assemblies @this,
		Func<Assembly, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<Assembly>(
			assembly => !predicate(assembly),
			$" except {doNotPopulateThisValue.TrimCommonWhiteSpace()}"));
}
