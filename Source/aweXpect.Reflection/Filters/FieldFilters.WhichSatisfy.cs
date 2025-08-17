using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filters for fields that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Fields WhichSatisfy(this Filtered.Fields @this,
		Func<FieldInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
