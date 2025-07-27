using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Methods WhichSatisfy(this Filtered.Methods @this,
		Func<MethodInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue} "));
}
