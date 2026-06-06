using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Excludes methods that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all async methods except specific ones.
	/// </remarks>
	public static Filtered.Methods Except(this Filtered.Methods @this,
		Func<MethodInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<MethodInfo>(
			method => !predicate(method),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
