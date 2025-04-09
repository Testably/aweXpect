using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Collections;

public static partial class FilteredExtensions
{
	/// <summary>
	///     Filters the assemblies according to the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Assemblies WhichSatisfy(this Filtered.Assemblies assemblies,
		Func<Assembly, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> assemblies.Which(Filter.Suffix(predicate, $" matching {doNotPopulateThisValue}"));
}
