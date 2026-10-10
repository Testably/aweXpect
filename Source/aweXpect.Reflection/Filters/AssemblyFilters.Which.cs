using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filters for assemblies that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Assemblies Which(this Filtered.Assemblies @this,
		Func<Assembly, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<Assembly>(assembly => UserCode.Invoke(predicate, assembly, "the predicate"),
			$" matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"));
}
