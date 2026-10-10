using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filters for methods that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Methods Which(this Filtered.Methods @this,
		Func<MethodInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<MethodInfo>(method => UserCode.Invoke(predicate, method, "the predicate"),
			$"matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
