using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filters for properties that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Properties Which(this Filtered.Properties @this,
		Func<PropertyInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<PropertyInfo>(property => UserCode.Invoke(predicate, property, "the predicate"),
			$"matching {doNotPopulateThisValue.TrimExpression()} "));
}
