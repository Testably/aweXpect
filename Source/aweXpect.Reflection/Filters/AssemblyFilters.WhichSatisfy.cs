using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class AssemblyFilters
{
	/// <summary>
	///     Filters for assemblies that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Assemblies WhichSatisfy(this Filtered.Assemblies @this,
		Func<Assembly, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix(predicate, $" matching {doNotPopulateThisValue}"));
}
